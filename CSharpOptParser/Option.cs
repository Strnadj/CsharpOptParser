using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpOptParser
{

    /// <summary>
    /// Class for storing objects and values 
    /// - contains shortcut, full, options and default values.
    /// 
    /// @author strnadj
    /// </summary>
    public class Option : IComparable<Option>
    {
        /// <summary>
        /// Type - default is required.
        /// </summary>
        private int type = OptParser.REQUIRED;

        /// <summary>
        /// Short option name.
        /// </summary>
        private char shortName = 'a';

        /// <summary>
        /// Full option name.
        /// </summary>
        private string fullName = "";

        /// <summary>
        /// Default value string.
        /// </summary>
        private string defaultValue = "";

        /// <summary>
        /// Position in option list.
        /// </summary>
        public readonly int POSITION;

        /// <summary>
        /// String description.
        /// </summary>
        private string description;

        /// <summary>
        /// Is value required?! (no value)
        /// </summary>
        private int requiredValue = OptParser.OPTION_NO_VALUE;

        /// <summary>
        /// Filled?! </summary>
        private bool filled = false;

        /// <summary>
        /// Value. </summary>
        private string value_Renamed = "";

        /// <summary>
        /// Default option with specification of required values.
        /// </summary>
        /// <param name="shortName"> Shortcut </param>
        /// <param name="fullName"> Full name </param>
        /// <param name="defaultValue"> Default value </param>
        /// <param name="type"> Option type </param>
        /// <param name="description"> Description </param>
        /// <param name="requiredValue"> Required value? </param>
        public Option(char shortName, string fullName, string defaultValue, int type, string description, int requiredValue)
        {
            this.shortName = shortName;
            this.fullName = fullName;
            this.defaultValue = defaultValue;
            this.type = type;
            this.POSITION = -1;
            this.description = description;
            this.requiredValue = requiredValue;
        }

        /// <summary>
        /// Default OPTIONAL option constructor.
        /// </summary>
        /// <param name="shortName"> Shortcut </param>
        /// <param name="fullName"> Full name </param>
        /// <param name="defaultValue"> Default value </param>
        /// <param name="type"> Option type </param>
        /// <param name="description"> Description </param>
        public Option(char shortName, string fullName, string defaultValue, int type, string description)
            : this(shortName, fullName, defaultValue, type, description, OptParser.OPTION_NO_VALUE)
        {
        }

        /// <summary>
        /// Path or expression option.
        /// </summary>
        /// <param name="fullName"> Full name </param>
        /// <param name="type"> Option type </param>
        /// <param name="defaultValue"> Default value </param>
        /// <param name="position"> Position </param>
        /// <param name="description"> Description </param>
        public Option(string fullName, int type, string defaultValue, int position, string description)
        {
            this.fullName = fullName;
            this.defaultValue = defaultValue;
            this.POSITION = position;
            this.description = description;
            this.type = type;
        }

        /// <summary>
        /// Is value required?
        /// </summary>
        /// <returns> True if its </returns>
        public virtual bool ValueRequired
        {
            get
            {
                return this.requiredValue == OptParser.OPTION_VALUE_IS_REQUIRED;
            }
        }

        /// <summary>
        /// Is option filled? </summary>
        /// <returns> True if its </returns>
        public virtual bool Filled
        {
            get
            {
                return this.filled;
            }
        }

        /// <summary>
        /// Set option as filled.
        /// </summary>
        public virtual void setFilled()
        {
            this.filled = true;
        }

        /// <summary>
        /// Return value (if is not filled return default value!)
        /// </summary>
        /// <returns> Value or default value </returns>
        public virtual string value()
        {
            if (!Filled)
            {
                return this.defaultValue;
            }
            return this.value_Renamed;
        }

        /// <summary>
        /// Return actual value (always return value no default!).
        /// </summary>
        /// <returns> String of actual value. </returns>
        public virtual string Value
        {
            get
            {
                return value_Renamed;
            }
            set
            {
                this.value_Renamed = value;
            }
        }


        /// <summary>
        /// Get type of option. </summary>
        /// <returns> Option type </returns>
        public virtual int Type
        {
            get
            {
                return this.type;
            }
        }

        /// <summary>
        /// Return option shortcut. </summary>
        /// <returns> Option shortcut </returns>
        public virtual char ShortName
        {
            get
            {
                return this.shortName;
            }
        }

        /// <summary>
        /// Return full name of option. </summary>
        /// <returns> full name of option </returns>
        public virtual string FullName
        {
            get
            {
                return this.fullName;
            }
        }

        /// <summary>
        /// Get description.
        /// </summary>
        /// <returns> Option description </returns>
        public virtual string Description
        {
            get
            {
                return this.description;
            }
        }

        /// <summary>
        /// Is option required? </summary>
        /// <returns> True if its </returns>
        public virtual bool Required
        {
            get
            {
                return this.type == OptParser.REQUIRED;
            }
        }

        /// <summary>
        /// Compare options for uniqueness in set.
        /// </summary>
        /// <returns> True if options are same  </returns>
        public virtual bool Equals(Option aOption)
        {
            if (this.shortName == aOption.ShortName || this.fullName == aOption.FullName)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Compare options.
        /// </summary>
        /// <returns> Messure of comparing  </returns>
        public virtual int CompareTo(Option o)
        {
            // First required than optional, first short name
            if (this.type != o.Type)
            {
                if (this.type == OptParser.REQUIRED)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else if (this.shortName != o.ShortName)
            {
                return (int)this.shortName - (int)o.ShortName;
            }
            else
            {
                return this.fullName.CompareTo(o.FullName);
            }
        }
    }
}
