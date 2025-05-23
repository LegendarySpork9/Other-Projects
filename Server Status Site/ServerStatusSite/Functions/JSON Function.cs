namespace ServerStatusSite.Functions
{
    public class JSONFunction
    {
        public string GetModelJSON(object model) => Newtonsoft.Json.JsonConvert.SerializeObject(model);

        public object GetModel(string json) => Newtonsoft.Json.JsonConvert.DeserializeObject(json);
    }
}
