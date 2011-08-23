using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.ClientData
{
    public static class Skills
    {
        static Dictionary<int, SkillEntry> _skills = new Dictionary<int,SkillEntry>();
        static bool _skillsLoaded = false;
        public static int UpdateCount = 0;

        public static Dictionary<int, SkillEntry> List
        {
            get
            {
                if (!_skillsLoaded)
                {
                    _skillsLoaded = true;
                    foreach (Data.Skill skill in Data.Skills.List)
                        _skills.Add(skill.ID, new SkillEntry(skill.ID, skill.Index, skill.UseButton, skill.Name, 0.0f, 0.0f, 0, 0.0f));
                }
                return _skills;
            }
        }

        public static SkillEntry SkillEntry(int skillID)
        {
            return List[skillID];
        }
    }

    public class SkillEntry
    {
        private int _id;
        private int _index;
        private bool _hasUseButton;
        private string _name;
        private float _value;
        private float _valueUnmodified;
        private byte _lockType;
        private float _cap;

        public int ID
        {
            get { return _id; }
            set { _id = value; Skills.UpdateCount++; }
        }
        public int Index
        {
            get { return _index; }
            set { _index = value; Skills.UpdateCount++; }
        }
        public bool HasUseButton
        {
            get { return _hasUseButton; }
            set { _hasUseButton = value; Skills.UpdateCount++; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; Skills.UpdateCount++; }
        }
        public float Value
        {
            get { return _value; }
            set { _value = value; Skills.UpdateCount++; }
        }
        public float ValueUnmodified
        {
            get { return _valueUnmodified; }
            set { _valueUnmodified = value; Skills.UpdateCount++; }
        }
        public byte LockType
        {
            get { return _lockType; }
            set { _lockType = value; Skills.UpdateCount++; }
        }
        public float Cap
        {
            get { return _cap; }
            set { _cap = value; Skills.UpdateCount++; }
        }

        public SkillEntry(int id, int index, bool useButton, string name, float value, float unmodified, byte locktype, float cap)
        {
            ID = id;
            Index = index;
            HasUseButton = useButton;
            Name = name;
            Value = value;
            ValueUnmodified = unmodified;
            LockType = locktype;
            Cap = cap;
        }
    }
}
