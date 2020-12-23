﻿using Es.Udc.DotNet.Amazonia.Model.LabelServiceImp.DTOs;
using System;
using System.Collections.Generic;

namespace Es.Udc.DotNet.Amazonia.Model.CommentServiceImp.DTOs
{
    [Serializable()]
    public class CommentDTO
    {
        public long id { get; set; }
        public string title { get; set; }
        public string value { get; set; }
        public System.DateTime date { get; set; }
        public long productId { get; set; }
        public long clientId { get; set; }
        public string clientName { get; set; }
        public List<LabelDTO> labels { get; set; }

        public CommentDTO()
        {
        }

        public CommentDTO(long id, string title, string value, DateTime date, long productId, long clientId, string clientName, List<LabelDTO> labels)
        {
            this.id = id;
            this.title = title;
            this.value = value;
            this.date = date;
            this.productId = productId;
            this.clientId = clientId;
            this.clientName = clientName;
            this.labels = labels;
        }

        public override bool Equals(object obj)
        {
            var dTO = obj as CommentDTO;
            return dTO != null &&
                   id == dTO.id &&
                   title == dTO.title &&
                   value == dTO.value &&
                   date == dTO.date &&
                   productId == dTO.productId &&
                   clientId == dTO.clientId &&
                   clientName == dTO.clientName &&
                   EqualityComparer<List<LabelDTO>>.Default.Equals(labels, dTO.labels);
        }

        public override int GetHashCode()
        {
            var hashCode = 1257116738;
            hashCode = hashCode * -1521134295 + id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(title);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(value);
            hashCode = hashCode * -1521134295 + date.GetHashCode();
            hashCode = hashCode * -1521134295 + productId.GetHashCode();
            hashCode = hashCode * -1521134295 + clientId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(clientName);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<LabelDTO>>.Default.GetHashCode(labels);
            return hashCode;
        }
    }
}
