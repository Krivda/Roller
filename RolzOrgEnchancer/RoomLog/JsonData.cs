﻿using System.Collections.Generic;
// ReSharper disable InconsistentNaming

namespace RolzOrgEnchancer.RoomLog
{
    //
    //json2csharp for https://rolz.org/api/roomlog?room=...
    //

    public class Data
    {
        public string creator { get; set; }
    }

    public class Room
    {
        public string key { get; set; }
        public string name { get; set; }
        public string created { get; set; }
        public Data data { get; set; }

    }

    public class Tag
    {
        public string k { get; set; }
        public string v { get; set; }
    }

    public class Item
    {
        public string type { get; set; }
        public string from { get; set; }
        public string text { get; set; }
        public string key { get; set; }
        public string h_time { get; set; }
        public long time { get; set; }
        public string room_id { get; set; }
        public string input { get; set; }
        public string stat { get; set; }
        public string result { get; set; }
        public string details { get; set; }
        public List<Tag> tags { get; set; }
        public string comment { get; set; }
        public string context { get; set; }
        public string style { get; set; }
        public List<string> items { get; set; }
        public string html { get; set; }
    }

    public class RootObject
    {
        public Room room { get; set; }
        public List<Item> items { get; set; }
    }

}