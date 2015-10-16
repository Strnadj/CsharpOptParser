using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpOptParser
{
    /// <summary>
    /// Generall option parser for definition and parsing command line arguments!
    /// 
    /// @author strnadj <jan.strnadek@gmail.com>
    /// 
    /// You can specify variable options, for presents or for boolean and default values!
    /// equal inputs are: -v,--verbose
    /// Inputs and factory method returns itself for fluent interfaces using it is recommended.
    /// Inspired by ruby-stdlib opt parser class
    /// (http://ruby-doc.org/stdlib-2.0.0/libdoc/optparse/rdoc/OptionParser.html)
    /// </summary>
    public class OptParser
    {
        /// <summary>
        /// Store options definitions in list. </summary>
        private ISet<Option> optionsContainer = new SortedSet<Option>();

        /// <summary>
        /// Store object map for quicker searching (after parsing arguments!). </summary>
        private IDictionary<string, Option> optionsValues = new Dictionary<string, Option>();

        /// <summary>
        /// Private data for string help methods etc.. </summary>
        private int maxFullNameLength = 0;

        /// <summary>
        /// Private variable for save option (path|expr) order. </summary>
        private int expressionOrder = 0;

        /// <summary>
        /// Help output optional string. </summary>
        private string exprHelpStringOptional = "";

        /// <summary>
        /// Help output required string. </summary>
        private string exprHelpStringRequired = "";

        /// <summary>
        /// List of required options in order row. </summary>
        private List<Option> exprRequiredOrder = new List<Option>();

        /// <summary>
        /// List of optional options in order row. </summary>
        private List<Option> exprOptionalOrder = new List<Option>();

        /// <summary>
        /// Command name for help method. </summary>
        private string commandName;

        /// <summary>
        /// Command description. </summary>
        private string commandDescription;

        /// <summary>
        /// Constants for optional type. </summary>
        public const int OPTIONAL = 1;

        /// <summary>
        /// Constants for required type. </summary>
        public const int REQUIRED = 2;

        /// <summary>
        /// Option - no value. </summary>
        public const int OPTION_NO_VALUE = 1;

        /// <summary>
        /// Option - value is required. </summary>
        public const int OPTION_VALUE_IS_REQUIRED = 2;

        /// <summary>
        /// Debug mode. </summary>
        public const bool DEBUG = false;

        /// <summary>
        /// Factory method returns instance of OptParser.
        /// </summary>
        /// <param name="cmdName"> Command name </param>
        /// <param name="cmdDesc"> Command description
        /// </param>
        /// <returns> Instance of option parser. </returns>
        public static OptParser createOptionParser(string cmdName, string cmdDesc)
        {
            return new OptParser(cmdName, cmdDesc);
        }


        /// <summary>
        /// Default constructor for option parser.
        /// </summary>
        /// <param name="name"> Command name </param>
        /// <param name="desc"> Command description </param>
        public OptParser(string name, string desc)
        {
            this.commandName = name;
            this.commandDescription = desc;
        }

        /// <summary>
        /// Add option to option parser, return self for fluent interface.
        /// </summary>
        /// <param name="shortName"> Shortcut </param>
        /// <param name="fullName"> Full name </param>
        /// <param name="type"> Optional or required? </param>
        /// <param name="defaultValue"> Default value </param>
        /// <param name="description"> Description
        /// </param>
        /// <returns> OptionParser </returns>
        public virtual OptParser addOption(char shortName, string fullName, int type, string defaultValue, string description)
        {
            return addOption(shortName, fullName, type, defaultValue, description, OptParser.OPTION_NO_VALUE);
        }


        /// <summary>
        /// Create option with required value.
        /// </summary>
        /// <param name="shortName"> Short name 'd' </param>
        /// <param name="fullName"> Full name 'directories' </param>
        /// <param name="type"> Type Required / Optional </param>
        /// <param name="defaultValue"> Default value (when value is not present) </param>
        /// <param name="description"> Command descritpion
        /// </param>
        /// <returns> OptionParser </returns>
        public virtual OptParser addOptionRequiredValue(char shortName, string fullName, int type, string defaultValue, string description)
        {
            return addOption(shortName, fullName, type, defaultValue, description, OptParser.OPTION_VALUE_IS_REQUIRED);
        }

        /// <summary>
        /// Add option.
        /// </summary>
        /// <param name="shortName"> Shortcut </param>
        /// <param name="fullName"> Full name </param>
        /// <param name="type"> Optional or required? </param>
        /// <param name="defaultValue"> Default value </param>
        /// <param name="description"> Description </param>
        /// <param name="valueType"> Value type
        /// </param>
        /// <returns> Option parser </returns>
        public virtual OptParser addOption(char shortName, string fullName, int type, string defaultValue, string description, int valueType)
        {
            optionsContainer.Add(new Option(shortName, fullName, defaultValue, type, description, valueType)
               );

            // Count variables of lenght for output
            if (fullName.Length > maxFullNameLength)
            {
                maxFullNameLength = fullName.Length;
            }

            return this;
        }

        /// <summary>
        /// Can add path or expression for example ls [path] or cp [path1] [path2].
        /// </summary>
        /// <param name="fullName"> Full name </param>
        /// <param name="type"> Optional or required </param>
        /// <param name="defaultValue"> Default value </param>
        /// <param name="description"> Description
        /// </param>
        /// <returns> Option parser instance </returns>
        public virtual OptParser addPathOrExpression(string fullName, int type, string defaultValue, string description)
        {
            Option o = new Option(fullName, type, defaultValue, expressionOrder++, description);
            optionsContainer.Add(o);

            // Helper string?!
            if (type == OptParser.REQUIRED)
            {
                exprRequiredOrder.Add(o);
                exprHelpStringRequired += "\"" + fullName + "\" ";
            }
            else
            {
                exprOptionalOrder.Add(o);
                exprHelpStringOptional += "[" + fullName + "] ";
            }

            return this;
        }


        /// <summary>
        /// Return command help created from options.
        /// </summary>
        /// <returns> Help string </returns>
        public virtual string Help
        {
            get
            {
                string ret = "";

                // name + description
                ret += string.Format("Command: {0} - {1}\n", this.commandName, this.commandDescription);

                // Usage?!
                // Better formmating
                if (exprHelpStringRequired.Length > 0)
                {
                    exprHelpStringRequired += " ";
                }
                ret += string.Format("Usage: {0} [options] {1}{2} \n\n", this.commandName, exprHelpStringRequired, exprHelpStringOptional);

                // Required parameters
                string required = "";

                // Optional parameters
                string optional = "";

                foreach (Option o in optionsContainer)
                {
                    // Params
                    if (o.POSITION != -1)
                    {
                        continue;
                    }

                    // Count necessary spaces for pretty output!
                    // -(char), --(string){spaces} - 3 + 3 + fullLenght
                    // {spaces} is variable maxFullNameLength + 4
                    int spaces = (maxFullNameLength + 4) - o.FullName.Length;
                    string freeSpaces = "";
                    for (int i = 0; i < spaces; i++)
                    {
                        freeSpaces += " ";
                    }

                    // Prepare value is required?!
                    string value = "";
                    if (o.ValueRequired)
                    {
                        value = "(Value is required!!)";
                    }

                    // Return
                    if (o.Type == OptParser.OPTIONAL)
                    {
                        optional += string.Format("\t-{0}, --{1}{2}{3} {4}\n", o.ShortName, o.FullName, freeSpaces, o.Description, value);
                    }
                    else
                    {
                        required += string.Format("\t-{0}, --{1}{2}{3} {4}\n", o.ShortName, o.FullName, freeSpaces, o.Description, value);
                    }
                }

                // Merge with return string
                if (required.Length > 0)
                {
                    ret += "Required options:\n" + required + "\n";
                }

                // Optional options
                if (optional.Length > 0)
                {
                    ret += "Optional options:\n" + optional + "\n";
                }

                return ret;
            }
        }

        /// <summary>
        /// Public method for parsing from collection of strings
        /// - String with parameters </summary>
        /// <param name="parameters"> String of parameters </param>
        /// <exception cref="Parsing"> exception </exception>
        public virtual void parseArguments(string parameters)
        {
            parseArguments(parameters.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Public method for parsing from collection of strings
        /// - collection must be LIST, cause parameters must be ordered! </summary>
        /// <param name="parameters"> List of arguments </param>
        /// <exception cref="Parsing"> exception </exception>
        public virtual void parseArguments(List<string> parameters)
        {
            string[] arr = new string[parameters.Count];
            for (int i = 0; i < parameters.Count; i++)
            {
                arr[i] = parameters[i];
            }
            parseArguments(arr);
        }

        /// <summary>
        /// Parse arguments from command line
        ///  - work with array is much more faster!
        ///  @params parameters Parameters </summary>
        ///  <exception cref="Parsing"> exception </exception>
        public virtual void parseArguments(string[] parameters)
        {
            /// <summary>
            /// Concatenate parameters - single and double quoted spaces etc </summary>
            parameters = concatenateParameters(parameters);

            // Method is designed to throw exception when is catch undefined attribute!
            // Non-optional items -  fill with required parameters, if at the end
            // of parsing there is some items - its bad and required parameters are missing
            ISet<string> nonOptionalItems = this.RequiredParameters;
            // Every time when parse some required option, this option are removed from set

            // Get count of parameters
            int size = parameters.Length;

            // Is there HELP option?!
            bool help = false;
            for (int i = 0; i < size; i++)
            {
                string parameter = parameters[i];

                // Choiced help option - ignore warnings about missing properties!
                if (parameter.Equals("-h") || parameter.Equals("--help"))
                {
                    help = true;

                    if (DEBUG)
                    {
                        Console.WriteLine("DEBUG: Help parameter found!");
                    }
                }
            }

            // Debug
            if (DEBUG)
            {
                Console.WriteLine("OptParser debug:\nDEBUG: Parameters " + parameters);
            }

            // Path or expressions on the end!
            List<string> poe = new List<string>();

            // Start parsing!
            for (int i = 0; i < size; i++)
            {
                // Always start with empty option! (for save on method end)
                Option o = null;

                // Get parameter
                string parameter = parameters[i];

                // What kind of parameter is it?!
                if (isOption(parameter))
                {
                    // Get option by parameter!
                    o = getOptionByParameter(parameter);

                    // Throw exception of undefined option!
                    if (o == null)
                    {
                        if (DEBUG)
                        {
                            Console.WriteLine(string.Format("ERROR: Command: {0} unexcepted option {1}", this.commandName, parameter));
                        }

                        throw new UnexpectedOption(string.Format("Command: {0} unexcepted option {1}", this.commandName, parameter));
                    }

                    // Debug
                    if (DEBUG)
                    {
                        Console.WriteLine("DEBUG: found parameter " + o.FullName);
                    }

                    // Set filled! 
                    o.setFilled();

                    // Required value?!
                    if (o.ValueRequired)
                    {
                        // Now we've got option, is there a value? on next index?!
                        string nextParameter = null;
                        if ((i + 1) < size)
                        {
                            nextParameter = parameters[i + 1];
                        }


                        // Throw new exception when next string is null (not exist) or next string is option!!!
                        if (nextParameter == null || isOption(nextParameter))
                        {
                            if (DEBUG)
                            {
                                Console.WriteLine(string.Format("DEBUG: Command: {0} - value: {1}({2}) is required!", this.commandName, o.ShortName, o.FullName));
                            }

                            // End with exception
                            throw new MissingOptionValue(string.Format("Command: {0} - value: {1}({2}) is required!", this.commandName, o.ShortName, o.FullName));
                        }

                        // Set next parameter as value!
                        o.Value = nextParameter;

                        if (DEBUG)
                        {
                            Console.WriteLine("DEBUG: Set value: " + nextParameter + " to command: " + o.FullName);
                        }

                        // Skip next parameter! (it is value for this option)
                        i = i + 1;
                    }

                    // Now if option is required remove from required list!
                    if (o.Required)
                    {
                        nonOptionalItems.Remove(o.FullName);
                    }
                }
                else
                {
                    // It is path or expr option! Parse different!!
                    poe.Add(parameter);

                    if (DEBUG)
                    {
                        Console.WriteLine("DEBUG: Add: '" + parameter + "' to path or expression");
                    }
                }

                // If there is an option save it!
                if (o != null)
                {
                    optionsValues[o.FullName] = o;
                }
            }

            if (DEBUG)
            {
                Console.WriteLine("DEBUG: Merge POE size:" + poe.Count);
            }

            // *************************************** //
            // Parse path or expressions parameters!!! //
            // *************************************** //
            // First concatenate parameters! (path \/s, ' and " open)
            // when there is opened " or ' - throws OptParser exception
            IDictionary<int?, string> poeParsed = new Dictionary<int?, string>();
            int actPoeIndex = 0;

            // Iterate through paths or expressions
            for (int i = 0; i < poe.Count; i++)
            {
                string pom = poe[i].Trim();

                // Skip when there is no string!
                if (pom.Length == 0)
                {
                    continue;
                }

                if (DEBUG)
                {
                    Console.WriteLine("Opt parser find POE: " + i + " - " + pom);
                }

                // Get param
                string param;
                if (pom[0] == '\'' || pom[0] == '"')
                {
                    param = pom.Substring(1, pom.Length - 1 - 1);
                }
                else
                {
                    param = pom;
                }

                if (DEBUG)
                {
                    Console.WriteLine("Find parameters: " + param);
                }

                // Set
                poeParsed[actPoeIndex] = param;
                actPoeIndex++;
            }

            if (DEBUG)
            {
                Console.WriteLine("DEBUG: Merge POE END");
            }


            // Set for items!
            int posRequired = 0;
            int posOptional = 0;

            // Try to assing values to options!
            for (int i = 0; i < actPoeIndex; i++)
            {
                string param = (string)poeParsed[i];
                // Try required first
                if (posRequired < exprRequiredOrder.Count)
                {
                    // Get option
                    Option o = exprRequiredOrder[posRequired];

                    // Set value
                    o.Value = param;
                    o.setFilled();

                    // Add to option values!
                    optionsValues[o.FullName] = o;

                    // Remove from required items!
                    nonOptionalItems.Remove(o.FullName);

                    // Move pos required
                    posRequired++;
                }
                else if (posOptional < exprOptionalOrder.Count)
                {
                    // Get option
                    Option o = exprOptionalOrder[posOptional];

                    // Set value
                    o.Value = param;
                    o.setFilled();

                    // Add to option values
                    optionsValues[o.FullName] = o;

                    // Inc
                    posOptional++;
                }
                else
                {
                    // Unknown attribute!
                    throw new UnknownAttribute(string.Format("Unknown attribute: \"{0}\" for command: {1}\n", param, this.commandName));
                }
            }

            // There is some required parameters left!
            if (nonOptionalItems.Count > 0)
            {
                string options = "";
                foreach (string s in nonOptionalItems)
                {
                    options = s + ", ";
                }

                options = options.Substring(0, options.Length - 2);

                // Throw exception
                if (help)
                {
                    throw new MissingOptionsHelp(this.Help);
                }
                else
                {
                    throw new MissingOptions(string.Format("Missing options for command: {0} - {1}\n{2}", this.commandName, options, this.Help));
                }
            }
        }

        /// <summary>
        /// Concatenate parameters quotes etc.
        /// </summary>
        /// <param name="arr"> Parameters array
        /// </param>
        /// <exception cref="Exception"> Parenthesses overleaps
        /// </exception>
        /// <returns> Concatened array </returns>
        private string[] concatenateParameters(string[] @params)
        {
            // List of new strings
            List<string> parameters = new List<string>();

            // Marks
            bool singleQuotedOpen = false;
            bool doubleQuotedOpen = false;


            string test = "";
            for (int i = 0; i < @params.Length; i++)
            {
                if (i == 0)
                {
                    test = @params[i];
                }
                else
                {
                    test += " " + @params[i];
                }
            }

            // Char by char and count \\
            int length = test.Length;
            int escapedCount = 0;
            string token = "";

            for (int i = 0; i < length; i++)
            {
                char c = test[i];
                if (c == ' ' && escapedCount % 2 == 0 && !singleQuotedOpen && !doubleQuotedOpen && token.Length > 0)
                {
                    if (token.Length > 0)
                    {
                        parameters.Add(token);
                    }
                    escapedCount = 0;
                    token = "";
                }
                else if (c == '\\')
                {
                    escapedCount++;
                    token += "\\";
                }
                else if (c == '\'' && escapedCount % 2 == 0)
                {
                    if (singleQuotedOpen == true)
                    {
                        token += "'";
                        if (token.Length > 0)
                        {
                            parameters.Add(token);
                        }
                        escapedCount = 0;
                        singleQuotedOpen = false;
                        token = "";
                    }
                    else if (doubleQuotedOpen)
                    {
                        throw new OverlapingBracketsException("Quoted \" overleaping with '!");
                    }
                    else
                    {
                        singleQuotedOpen = true;
                        if (token.Length > 0)
                        {
                            parameters.Add(token);
                        }
                        escapedCount = 0;
                        token = "'";
                    }
                }
                else if (c == '"' && escapedCount % 2 == 0)
                {
                    if (doubleQuotedOpen)
                    {
                        token += "\"";
                        if (token.Length > 0)
                        {
                            parameters.Add(token);
                        }
                        escapedCount = 0;
                        doubleQuotedOpen = false;
                        token = "";
                    }
                    else if (singleQuotedOpen)
                    {
                        throw new OverlapingBracketsException("Quote ' overleaping with \"!");
                    }
                    else
                    {
                        doubleQuotedOpen = true;
                        if (token.Length > 0)
                        {
                            parameters.Add(token);
                        }
                        escapedCount = 0;
                        token = "\"";
                    }
                }
                else
                {
                    token += c + "";
                    escapedCount = 0;
                }
            }

            if (token.Length > 0)
            {
                parameters.Add(token);
            }
            if (singleQuotedOpen)
            {
                throw new OverlapingBracketsException("Single quoted bracket not closed!");
            }

            if (doubleQuotedOpen)
            {
                throw new OverlapingBracketsException("Double quoted bracket not closed!");
            }

            string[] arr = new string[parameters.Count];
            for (int i = 0; i < parameters.Count; i++)
            {
                arr[i] = parameters[i];
            }

            return arr;
        }

        /// <summary>
        /// Return true when parametr string is an option!
        /// </summary>
        /// <param name="parameter"> String parameter
        /// </param>
        /// <returns> True if it is option. </returns>
        private bool isOption(string parameter)
        {
            return (parameter.StartsWith("-", StringComparison.Ordinal) || parameter.StartsWith("--", StringComparison.Ordinal));
        }


        /// <summary>
        /// Return option by parameter.
        /// </summary>
        /// <param name="parameter"> Parameter name
        /// </param>
        /// <returns> Option instance </returns>
        private Option getOptionByParameter(string parameter)
        {
            // Compare with what?!
            if (parameter.StartsWith("--", StringComparison.Ordinal))
            {
                // Remove -- from parameter name
                parameter = parameter.Substring(2);

                // Full
                foreach (Option o in optionsContainer)
                {
                    if (o.FullName.Equals(parameter))
                    {
                        return o;
                    }
                }
            }
            else
            {
                // Short name!
                char param = parameter[1];

                // Compare
                foreach (Option o in optionsContainer)
                {
                    if (o.ShortName == param)
                    {
                        return o;
                    }
                }
            }

            // Nothing was found!
            return null;
        }

        /// <summary>
        /// Return default value or filled value from parameters!
        /// (we are not able decide if option was set or not from this method)
        /// </summary>
        /// <param name="parameter"> Option name
        /// </param>
        /// <returns> Value </returns>
        public virtual string getOptionValue(string parameter)
        {
            // --
            if (!parameter.StartsWith("--", StringComparison.Ordinal))
            {
                parameter = "--" + parameter;
            }

            // Get option
            Option o = getOptionByParameter(parameter);

            if (o == null)
            {
                return null;
            }

            return o.value();
        }

        /// <summary>
        /// Return true when option is filled!
        /// </summary>
        /// <param name="optName"> Option name
        /// </param>
        /// <returns> True when option is filled </returns>
        public virtual bool isOptionFilled(string optName)
        {
            if (optionsValues.ContainsKey(optName))
            {
                Option o = optionsValues[optName];
                return o.Filled;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get option via full name. </summary>
        /// <param name="optName"> Option name </param>
        /// <returns> Option instance or null </returns>
        public virtual Option getOption(string optName)
        {
            if (optionsValues.ContainsKey(optName))
            {
                Option o = optionsValues[optName];
                return o;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Return all required parameters in set.
        /// </summary>
        /// <returns> Set of required parameters </returns>
        public virtual ISet<string> RequiredParameters
        {
            get
            {
                ISet<string> ret = new SortedSet<string>();

                // Iterate parameters
                foreach (Option o in optionsContainer)
                {
                    if (o.Required)
                    {
                        ret.Add(o.FullName);
                    }
                }

                // Iterate required parameters (paths or expressions)
                foreach (Option o in exprRequiredOrder)
                {
                    if (o.Required)
                    {
                        ret.Add(o.FullName);
                    }
                }

                // return
                return ret;
            }
        }
    }
}
