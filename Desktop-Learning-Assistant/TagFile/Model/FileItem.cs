﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Model
{
    /// <summary>
    /// 文件实体类
    /// </summary>
    public class FileItem
    {
        public int FileItemId { get; set; }

        public string DisplayName { get; set; }

        public string RealName { get; set; }

        public virtual ICollection<TagFileRelation> Relations { get; private set; }
            = new ObservableCollection<TagFileRelation>();

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"File DisplayName: {DisplayName}, File Id: {FileItemId}\n");
            sb.Append($"Relations of {DisplayName}:\n");
            foreach (var relation in Relations)
            {
                sb.Append($"Tag Name: {relation.Tag.TagName}, " +
                          $"Local Create Time: {relation.LocalCreateTime}\n");
            }
            return sb.ToString();
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var other = (FileItem)obj;
            if (FileItemId == other.FileItemId)
            {
                Debug.Assert(DisplayName == other.DisplayName && RealName == other.RealName);
                return true;
            }
            return false;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return FileItemId ^ DisplayName.GetHashCode() ^ RealName.GetHashCode();
        }
    }
}
