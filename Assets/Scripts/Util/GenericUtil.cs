namespace Util {
    public static class GenericUtil {
        public static object GetPropValue(this object obj, string name) {
            foreach (var part in name.Split('.')) {
                if (obj == null) {
                    return null;
                }

                var type = obj.GetType();
                var infoP = type.GetProperty(part);
                var infoF = type.GetField(part);

                if (infoF != null) {
                    obj = infoF.GetValue(obj);
                }

                if (infoP != null) {
                    obj = infoP.GetValue(obj, null);
                }
            }
            return obj;
        }

        public static T GetPropValue<T>(this object obj, string name) {
            var retval = GetPropValue(obj, name);
            if (retval == null) {
                return default(T);
            }

            // throws InvalidCastException if types are incompatible
            return (T) retval;
        }
    }
}