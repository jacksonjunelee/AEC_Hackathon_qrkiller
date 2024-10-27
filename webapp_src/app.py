import viktor as vkt
import json
import rhino3dm
from pathlib import Path


def options(params, **kwargs):
    return [

    ]

class Parametrization(vkt.Parametrization):
    intro = vkt.Text("## Generative tower \n This app parametrically generates and visualizes a 3D tower using a Grasshopper script. \n\n Please select your option of choice:")

    # Input fields
    
    Seed = vkt.OptionField('Option', options=[
        vkt.OptionListElement(0, '1'),
        vkt.OptionListElement(1, '2'),
        vkt.OptionListElement(2, '3'),
        vkt.OptionListElement(3, '4'),
        vkt.OptionListElement(4, '5'),
        vkt.OptionListElement(5, '6'),
        vkt.OptionListElement(6, '7'),
        vkt.OptionListElement(7, '8'),
        vkt.OptionListElement(8, '9'),
        vkt.OptionListElement(9, '10'),
    ])

@vkt.memoize
def get_geometry(params):
    grasshopper_script_path = Path(__file__).parent / "10options.gh"
    script = vkt.File.from_path(grasshopper_script_path)
    input_parameters = dict(params)

    # Run the Grasshopper analysis and obtain the output data
    analysis = vkt.grasshopper.GrasshopperAnalysis(script=script, input_parameters=input_parameters)
    analysis.execute(timeout=300)
    output = analysis.get_output()

    return output

class Controller(vkt.Controller):
    parametrization = Parametrization

    @vkt.GeometryView("Geometry", duration_guess=10, x_axis_to_right=True, update_label='Run Grasshopper')
    def run_grasshopper(self, params, **kwargs):

        vkt.progress_message(message=f'Calculating new configuration')

        output = get_geometry(dict(params))

        # Convert output data to mesh
        file3dm = rhino3dm.File3dm()
        obj = rhino3dm.CommonObject.Decode(json.loads(output["values"][0]["InnerTree"]['{0}'][0]["data"]))
        file3dm.Objects.Add(obj)
        
        # Write to geometry_file
        geometry_file = vkt.File()
        file3dm.Write(geometry_file.source, version=7)

        geometry_file

        return vkt.GeometryResult(geometry=geometry_file, geometry_type="3dm")
