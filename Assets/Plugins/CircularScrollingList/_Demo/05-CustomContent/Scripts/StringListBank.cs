using System;
using System.Collections.Generic;
using AirFishLab.ScrollingList.ContentManagement;
using UnityEngine;

namespace AirFishLab.ScrollingList.Demo
{
    public class StringListBank : BaseListBank
    {
        [SerializeField] private List<StringListContent> _datas = new List<StringListContent>();

        public override IListContent GetListContent(int index)
        {
            return _datas[index];
        }

        public override int GetContentCount()
        {
            return _datas.Count;
        }
    }

    [Serializable]
    public class StringListContent : IListContent
    {
        [SerializeField] private string _value;

        public string Value => _value;
    }
}
