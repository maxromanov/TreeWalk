
import json
import jsonschema
import logging
 
logger = logging.getLogger(__name__)

# region JSting
class JString(object):
    def __init__(self,**kwargs):
        self._val = ''

    def __str__(self):
        return str(self._val)

    def _setValue(self,value):
        self._val = value 

    def _isEmpty(self):
        return ( self._val == '')
# endregion

# region JArray
class JArray(list):
    def __init__(self,**kwargs):
        if kwargs is not None:
            if kwargs is not None:
                if( kwargs.has_key('itemCls') ):
                    self.itemCls = kwargs.get('itemCls')
                    kwargs.pop('itemCls')
        super(JArray, self).__init__(**kwargs)

    def _isEmpty(self):
        return (len(self) == 0)
# endregion

# region JObject
class JObject(object):
    def __init__(self,**kwargs):
       pass

    def __setattr__(self, arg, value):
        for attr, val in self.__dict__.iteritems():
            if( attr == arg ):
                if(isinstance(val,JString)):
                    return val._setValue(value)
                elif(isinstance(val,JMixed)):
                    return val._setValue(value)
        return super(JObject, self).__setattr__(arg, value)

    def __iter__(self):
        for attr, val in self.__dict__.items():
            if( isinstance(val,JString) and not val._isEmpty() ):
                yield attr, val
            elif(isinstance(val,JArray) and not val._isEmpty()):
                yield attr,val 
            elif(isinstance(val,JMixed) and not val._isEmpty()):
                yield attr,val 
            elif(isinstance(val,JObject) and not val._isEmpty()):
                yield attr,val 

    def _isEmpty(self):
        result = True;
        for attr, val in self.__dict__.items():
            if( isinstance(val,JString) ):
                result = val._isEmpty()
            elif(isinstance(val,JArray) ):
               result = val._isEmpty()
            elif(isinstance(val,JMixed) ):
               result = val._isEmpty()
            elif(isinstance(val,JObject)):
               result =  val._isEmpty()
            if(not result ):
                return False
        return result

# endregion 

# region JNumber
class JNumber:
    def __init__(self,**kwargs):
        pass
# endregion

# region JMixed
class JMixed(object):
    def __init__(self,**kwargs):
        self._val = ''
    
    def _setValue(self,value):
        self._val = value
    
    def __str__(self):
        return str(self._val)

    def _isEmpty(self):
        return (self._val == '')

# endregion

class JSONModelEncoder(json.JSONEncoder):

    def default(self, o):
        if isinstance(o, JArray):
            return list(o)
        if isinstance(o,JObject):
            return dict(o)
        if isinstance(o,JString):
            return str(o)
        if isinstance(o,JMixed):
            return str(o)
        return super(JSONModelEncoder, self).default(o)
