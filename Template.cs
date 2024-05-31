namespace PwshGen
{
    internal struct Template(string paramName, string process, string creatorName, DateTime createdDate)
    {
        public string requires = "#requires -version 7";
        public string help = string.Format("\n<#\n.SYNOPSIS\n  <>\n\n.DESCRIPTION\n  <>\n\n.PARAMETER  <{0}>\n  <>\n\n.INPUTS\n  <>\n\n.OUTPUTS\n  <>\n\n.NOTES\n  Version:\t\t1.0\n  Author:\t\t{1}\n  Creation Date:\t{2}\n  Purpose/Change: Init\n\n.EXAMPLE\n  <>\n#>", paramName, creatorName, createdDate.ToString("d"));
        public string cmdletBinding = "\n[CmdletBinding()]";
        public string scriptParams = string.Format("\nparam(\n\t[string]${0})", paramName);
        public string subFunction = string.Format("\nfunction FunctionName {{\n\tparam(\n\t\t[string]$FunctionParam\n\t){0}}}\n", process);
        public string mainFunction = string.Format("\nfunction Main {{\n\tFunctionName -FunctionParam ${0}\n}}", paramName);
        
        public string run = "\nMain\nexit 0";
    }
}
