namespace ServerStatusSite.Functions
{
    public class JSONFunction
    {
        // Converts the model to a JSON string.
        public string GetModelJSON(object model) => Newtonsoft.Json.JsonConvert.SerializeObject(model);

        // Converts the JSON string to an object.
        public object GetModel(string json) => Newtonsoft.Json.JsonConvert.DeserializeObject(json);
    }
}
