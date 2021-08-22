﻿using ADOTabular;
using ADOTabular.Interfaces;
using DaxStudio.UI.Enums;
using DaxStudio.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;
using Caliburn.Micro;
using DaxStudio.UI.Events;
using System.Collections.Specialized;

namespace DaxStudio.UI.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class QueryBuilderFilterList :  
        PropertyChangedBase,
        IQueryBuilderFieldList
    {
        public QueryBuilderFilterList(IEventAggregator eventAggregator,Func<IModelCapabilities> modelCapabilities)
        {
            EventAggregator = eventAggregator;
            DropHandler = new QueryBuilderDropHandler(this);
            GetModelCapabilities = modelCapabilities;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChange(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, e);
        }

        public void Remove(QueryBuilderFilter item)
        {
            Items.Remove(item);
            OnCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            NotifyOfPropertyChange(nameof(Items));
        }

        [JsonProperty]
        public ObservableCollection<QueryBuilderFilter> Items { get; } = new ObservableCollection<QueryBuilderFilter>();
        public IEventAggregator EventAggregator { get; }
        public QueryBuilderDropHandler DropHandler { get; }
        public Func<IModelCapabilities> GetModelCapabilities { get; }

        public IEnumerable<FilterType> FilterTypes
        {
            get
            {
                var items = Enum.GetValues(typeof(FilterType)).Cast<FilterType>();
                return items;
            }
        }



        #region IQueryBuilderFieldList
        public void Add(IADOTabularColumn item)
        {
            try
            {
                var filter = new QueryBuilderFilter(item, GetModelCapabilities(), EventAggregator);
                Items.Add(filter);
                OnCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
                NotifyOfPropertyChange(nameof(Items));
            }
            catch (Exception ex)
            {
                var msg = $"Error adding Filter to Query Builder: {ex.Message}";
                Log.Error(ex, Common.Constants.LogMessageTemplate, nameof(QueryBuilderFilterList), nameof(Add), msg);
                EventAggregator.PublishOnUIThreadAsync(new OutputMessage(MessageType.Error, msg));
            }
        }

        internal void Add(QueryBuilderFilter filter)
        {
            Items.Add(filter);
            OnCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, filter));
            NotifyOfPropertyChange(nameof(Items));
        }

        public bool Contains(IADOTabularColumn item)
        {
            return Items.FirstOrDefault(f => f.TabularObject == item) != null;
        }
        public int Count => Items.Count;

        public int IndexOf(IADOTabularColumn obj)
        {
            var item = Items.FirstOrDefault(f => f.TabularObject == obj);
            return Items.IndexOf(item);
        }
        public void Insert(int index, IADOTabularColumn item)
        {
            var filter = new QueryBuilderFilter(item, GetModelCapabilities(),EventAggregator);
            // if we are 'inserting' at the end just do an add
            if (index >= Items.Count) Items.Add(filter);
            Items.Insert(index, filter);
            OnCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }
        public void Move(int oldIndex, int newIndex)
        {
            Items.Move(oldIndex, newIndex);
            OnCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, Items, newIndex,oldIndex));
        }
        #endregion

        public void Clear()
        {
            Items.Clear();
            OnCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
