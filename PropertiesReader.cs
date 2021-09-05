using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static System.Convert;

namespace PropertiesReader {
  class Properties {
    private static Regex regDouble = new Regex("^\\d+,\\d+$");
    private static Regex regNumber = new Regex("^\\d+$");
    private static Regex regBoolean = new Regex("^((true)|(false))$");
    private static Regex regString = new Regex("^\".+\"$");
    private Dictionary<string, Dictionary<string, object>> properties = new Dictionary<string, Dictionary<string, object>>();
    public Properties() {
      properties.Add("root", new Dictionary<string, object>());
    }
    public Properties(string str) {
      properties.Add("root", new Dictionary<string, object>());
      string root = "root";
      foreach(string line in str.Split('\n')) {
        string key = line.Split('=')[0].Trim(); 
        if(!Regex.IsMatch(line, @"^\[\w+\]$")) {
          dynamic value = line.Split('=')[1].Trim();
          if(value.Length < 1)throw new MissingFieldException($"{key} is empty");
          // Console.WriteLine("[{0}] {1} = {2}", root, key, value);
          if(regDouble.IsMatch(value)) {
            value = Convert.ToDouble(value);
          } else if(regNumber.IsMatch(value)) {
            value = Convert.ToInt32(value);
          } else if(regBoolean.IsMatch(value)) {
            value = Convert.ToBoolean(value);
          } else if(regString.IsMatch(value)) {
            value = value.Substring(1, value.Length-2);
          } else value = null;
          this.set(root, key, value);
        } else root = key.Substring(1, key.Length-2);
      }
    }
    public void set(string property, dynamic value) {
      set("root", property, value);
    }
    public void set(string root, string property, dynamic value) {
      if(!properties.ContainsKey(root)) {
        properties.Add(root, new Dictionary<string, object>());
      }
      if(properties[root].ContainsKey(property)) {
        properties[root][property] = value;
      } else properties[root].Add(property, value);
    }
    public Dictionary<string, Dictionary<string, object>> get() {
      return properties;
    }
    public dynamic get(string property) {
      return get("root", property);
    }
    public dynamic get(string root, string property) {
      if(!properties.ContainsKey(root) || !properties[root].ContainsKey(property))return null;
      return properties[root][property];
    }
    public bool has(string property) {
      return has("root", property);
    }
    public bool has(string root, string property) {
      return properties.ContainsKey(root) && properties[root].ContainsKey(property);
    }

    public override string ToString()
    {
      string str = "Properties:";
      foreach(KeyValuePair<string, Dictionary<string, object>> i in properties) {
        str += $"\n [{i.Key}]";
        foreach(KeyValuePair<string, object> j in i.Value) {
          str += $"\n  {j.Key} = {j.Value}";
        }
      }
      return str;
    }
  }
  class ReaderProperties {
    static public Properties read(string path) {
      if(!File.Exists(path))throw new Exception($"{path} not found");
      using(StreamReader sr = new StreamReader(path)) {
        return new Properties(sr.ReadToEnd());
      }
    }
  }
  // class program {
  //   static public void Main(string[] args) {Console.WriteLine(ReaderProperties.read("./Test.properties"));}
  // }
}