using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Be.Corebvba.Aubergine.Interfaces;

namespace Be.Corebvba.Aubergine.Model
{
    public class SpecElement : ISpecElement 
    {

        #region IElement Members

        public string Description {get ; set;}
        bool? status = true;
        public bool? Status { 
            get {
                if (status != null)
                {
                    foreach (var x in Children)
                    {
                        if (x.Status.HasValue == false)
                        {
                            status = null;
                            break;
                        }
                        if (x.Status == false)
                            status = false;
                    }
                }
                return status;
            }
            set
            {
                status = value;
            }
        }

        public IEnumerable<ISpecElement> children = new ISpecElement[] {};
        public IEnumerable<ISpecElement> Children
        {
            get {
                foreach (var c in children)
                    c.Parent = this;
                return children; 
            }
            set
            {
                children = value;
            }
        }
        public string StatusInfo { get; set; }

        public ElementType Type {get;private set;}

        public ISpecElement Parent {get;set;}

        public SpecElement(ElementType type, string description) 
        {
            this.Type = type;
            this.Description = description;
            this.status = null;
        }

        public string StatusText
        {
            get {
                switch (Status)
                {
                    case null:
                        return "IMPLEMENTATION ERROR";
                        break;
                    case true:
                        return "OK";
                        break;
                    case false:
                        return "NOK";
                        break;
                }
                return "UNKNOWN";
            }
        }

        public ISpecElement Clone()
        {
            var x = new SpecElement(this.Type,this.Description);
            x.Children = new List<ISpecElement>(Children.Select(c=>c.Clone()));
            x.Parent = this.Parent;
            x.Status = this.Status;
            x.StatusInfo = this.StatusInfo;
            return x;
        }

        #endregion
    }
}
