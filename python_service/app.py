import viktor as vkt


class Parametrization(vkt.Parametrization):
    btn = vkt.SetParamsButton('execute this', 'endpoint1')
    pass # Welcome to VIKTOR! You can add your input fields here. Happy Coding!
    a = vkt.NumberField('test')
    c = vkt.HiddenField('yesyes')


class Controller(vkt.Controller):
    parametrization = Parametrization

    def endpoint1(self, params, **kwargs):
        print('running endpoint 1')
        return vkt.SetParamsResult({'a': 1, 'b':2, 'c': 'hellothere'})
