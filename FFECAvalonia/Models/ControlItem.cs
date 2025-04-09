using System;
namespace FFECAvalonia.Models
{
    public class ControlItem
    {
        private JObject _data = new JObject();
        public JObject Data 
        {
            get => _data; 
            set => _data = value;
        } // Position : List<int>, Test: string, StorageType: string
        public string Text
        {
            get 
            { 
                if (_data.ContainsKey("Text")) return _data["Text"].Value<string>(); 
                else return string.Empty;
            }
            set { _data["Text"] = value; }
        }
        public string StorageType
        {
            get 
            { 
                if (_data.ContainsKey("StorageType")) return _data["StorageType"].Value<string>(); 
                else return string.Empty;
            }
            set { _data["StorageType"] = value; }
        }
        public Dictionary<string, int> Position
        {
            get
            {
                if (_data.ContainsKey("Position")) return _data["Position"].ToObject<Dictionary<string, int>>(); 
                else return new Dictionary<string, int> {{"x", 0}, {"y", 0}};
            }
            set
            {
                JObject position = new JObject{
                    {"x", value["x"]},
                    {"y", value["y"]}
                };
                _data["Position"] = position;
            }
        }

        public List<int> PositionList
        {
            get { return new List<int>() {Position["x"], Position["y"]}; }
        }
        public string Sector
        {
            get 
            { 
                if (_data.ContainsKey("Sector")) return _data["Sector"].Value<string>(); 
                else return string.Empty;
            }
            set { _data["Sector"] = value; }
        }
        public string Name
        {
            get 
            { 
                if (_data.ContainsKey("Name")) return _data["Name"].Value<string>(); 
                else return string.Empty;
            }
            set { _data["Name"] = value; }
        }
    }
}