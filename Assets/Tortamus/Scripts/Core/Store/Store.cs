using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

public class Store
{
    public string Namespace { get; set; }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public void ClearNamespace(string _namespace)
    {
        throw new NotImplementedException();
    }

    public void SetValue(string key, string value)
    {
        SetValue(Namespace, key, value);
    }

    public void SetValue(string _namespace, string key, string value)
    {
    }

    public string GetValue(string key)
    {
        return GetValue(Namespace, key);
    }

    public string GetValue(string _namespace, string key)
    {
        return string.Empty;
    }

    public Store()
    {
        Namespace = string.Empty;
    }
}
