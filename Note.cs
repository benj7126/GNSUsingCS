using GNSUsingCS.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS
{
    internal class Note : Element
    {
        internal Dictionary<(FieldInfo, object), object> fieldsAndValues = new Dictionary<(FieldInfo, object), object>();

        public void UpdateValues()
        {
            fieldsAndValues = GetFields();
        }

        private void extractFields(object obj, Dictionary<(FieldInfo, object), object> fields)
        {
            foreach (FieldInfo field in obj.GetType().GetFields())
            {
                if (field.FieldType == typeof(string))
                {
                    fields.Add((field, obj), new string((string)field.GetValue(obj)));
                    continue;
                }

                if (field.FieldType.IsValueType)
                {
                    fields.Add((field, obj), field.GetValue(obj));
                }
                else
                {
                    extractFields(field.GetValue(obj), fields);
                }
            }

            if (obj is Element e)
                e.Children.ForEach(child => extractFields(child, fields));
        }

        public virtual Dictionary<(FieldInfo, object), object> GetFields()
        {
            Dictionary<(FieldInfo, object), object> fields = new Dictionary<(FieldInfo, object), object>();

            extractFields(this, fields);

            return fields;
        }

        public void SetValues()
        {
            foreach (KeyValuePair<(FieldInfo, object), object> fieldAndValue in fieldsAndValues)
            {
                if (!fieldAndValue.Key.Item1.GetValue(fieldAndValue.Key.Item2).Equals(fieldAndValue.Value))
                    fieldAndValue.Key.Item1.SetValue(fieldAndValue.Key.Item2, fieldAndValue.Value);
            }
        }
    }

    internal class NoteRef : Element
    {
        internal Dictionary<(FieldInfo, object), object> fieldsAndValues = new Dictionary<(FieldInfo, object), object>();

        string targetID;
        
        public NoteRef(string targetID) {
            this.targetID = targetID;

            Element e = ObjectIDController.Element.Get(targetID);

            if (e is Note note)
            {
                // fieldsAndValues = note.GetFields();

                // load values
            }
            else
                throw new Exception("Tried to create new note ref from non-note");
        }

        internal override void Update()
        {
            Element e = ObjectIDController.Element.Get(targetID);

            setValues();
            e.Update();
            returnValues();
        }

        internal override void Draw()
        {
            Element e = ObjectIDController.Element.Get(targetID);

            setValues();
            e.Draw();
            returnValues();
        }

        internal override void Recalculate(int x, int y, int w, int h)
        {
            Element e = ObjectIDController.Element.Get(targetID);

            setValues();
            e.Recalculate(x, y, w, h);
            returnValues();
        }

        /*
        private void setValue()
        {
            if (ObjectIDController.Element.Get(targetID) is Note note)
            {
                note.UpdateValues(); // should be done only when something actually changed...
                SetValues();
            }
        }
        */

        private void setValues()
        {
            if (ObjectIDController.Element.Get(targetID) is Note note)
            {
                foreach (KeyValuePair<(FieldInfo, object), object> fieldAndValue in note.fieldsAndValues)
                {
                    // save value of orig if it has been changed
                    object actualValue = fieldAndValue.Key.Item1.GetValue(fieldAndValue.Key.Item2);
                    if (!actualValue.Equals(fieldAndValue.Value))
                    {
                        note.fieldsAndValues[fieldAndValue.Key] = actualValue;
                    }

                    // use own value
                    if (fieldsAndValues.ContainsKey(fieldAndValue.Key))
                        fieldAndValue.Key.Item1.SetValue(fieldAndValue.Key.Item2, fieldsAndValues[fieldAndValue.Key]);
                }
            }
        }

        private void returnValues()
        {
            if (ObjectIDController.Element.Get(targetID) is Note note)
            {
                foreach (KeyValuePair<(FieldInfo, object), object> noteFieldAndValue in note.fieldsAndValues)
                {
                    object actualValue = noteFieldAndValue.Key.Item1.GetValue(noteFieldAndValue.Key.Item2);
                    if (!actualValue.Equals(noteFieldAndValue.Value))
                    {
                        if (!fieldsAndValues.ContainsKey(noteFieldAndValue.Key))
                            fieldsAndValues.Add(noteFieldAndValue.Key, actualValue);
                        else
                            fieldsAndValues[noteFieldAndValue.Key] = actualValue;

                        noteFieldAndValue.Key.Item1.SetValue(noteFieldAndValue.Key.Item2, noteFieldAndValue.Value);
                    }
                }
            }
        }
    }
}
