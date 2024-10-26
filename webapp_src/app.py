import viktor as vkt
from pathlib import Path


class Parametrization(vkt.Parametrization):
    pass # Welcome to VIKTOR! You can add your input fields here. Happy Coding!
    field = vkt.NumberField('example number')
    file = vkt.FileField('example file upload')


class Controller(vkt.Controller):
    parametrization = Parametrization

    @vkt.IFCView('IFC view')
    def get_ifc_view(self, params, **kwargs):
        ifc = vkt.File.from_path(Path(__file__).parent / 'sample.ifc')
        return vkt.IFCResult(ifc)
    
