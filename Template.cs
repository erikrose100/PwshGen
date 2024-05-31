namespace PwshGen
{
    internal struct Template(string paramName, string process)
    {
        public string requires = "#requires -version 7";
        public string help = string.Format("<#\n.SYNOPSIS\n  <>\n\n.DESCRIPTION\n  <>\n\n.PARAMETER  <{0}>\n  <>\n\n.INPUTS\n  <>\n\n.OUTPUTS\n  <>\n\n.NOTES\n  Version:\t\t1.0\n  Author:\t\t<Name>\n  Creation Date:\t<Date>\n  Purpose/Change: Init\n\n.EXAMPLE\n  <>\n#>", paramName);
        public string cmdletBinding = "[CmdletBinding()]\n";
        public string scriptParams = string.Format("[CmdletBinding()]\nparam(\n\t[string]${0}\n)", paramName);
        public string subFunction = string.Format("function FunctionName {{\n\tparam(\n\t\t[string]$FunctionParam\n\t){0}}}\n", process);
        public string mainFunction = string.Format("function Main {{\n\tFunctionName -FunctionParam ${0}\n}}", paramName);
        
        public string run = "\nMain\nexit 0";
        // public string process = "\n\n\tbegin {\n\t}\n\n\tprocess {\n\t}\n\n\tend {\n\t}";
    }

    // internal class Template
    // {
    //     public string? requires;
    //     public string? help;
    //     public string? cmdletBinding;
    //     public string? scriptParams;
    //     public string? subFunction;
    //     public string? mainFunction;
    //     public string? run;
    // }
}
