import viktor as vkt
import json
import rhino3dm
from pathlib import Path


class Parametrization(vkt.Parametrization):
    intro = vkt.Text("## Grasshopper app \n This app parametrically generates and visualizes a 3D model of a box using a Grasshopper script. \n\n Please fill in the following parameters:")

    # Input fields
    width = vkt.NumberField('Width', default=5)
    length = vkt.NumberField('Length', default=6)
    height = vkt.NumberField('Height', default=7)


class Controller(vkt.Controller):
    parametrization = Parametrization

    @vkt.GeometryView("Geometry", duration_guess=10, x_axis_to_right=True, update_label='Run Grasshopper')
    def run_grasshopper(self, params, **kwargs):
        grasshopper_script_path = Path(__file__).parent / "sample_box_grasshopper2.gh"
        script = vkt.File.from_path(grasshopper_script_path)
        input_parameters = dict(params)

        # Run the Grasshopper analysis and obtain the output data
        analysis = vkt.grasshopper.GrasshopperAnalysis(script=script, input_parameters=input_parameters)
        analysis.execute(timeout=30)
        output = analysis.get_output()
        
        # Convert output data to mesh
        file3dm = rhino3dm.File3dm()
        obj = rhino3dm.CommonObject.Decode(json.loads(output["values"][0]["InnerTree"]['{0}'][0]["data"]))
        file3dm.Objects.Add(obj)
        
        # Write to geometry_file
        geometry_file = vkt.File()
        file3dm.Write(geometry_file.source, version=7)
        return vkt.GeometryResult(geometry=geometry_file, geometry_type="3dm")
