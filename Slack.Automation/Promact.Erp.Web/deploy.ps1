$path = (Get-Item -Path ".\Web.config" -Verbose).FullName;
$xml = [xml](Get-Content $path);
$alldependentAssemblyNodes = $xml.configuration.runtime.assemblyBinding.dependentAssembly;
foreach($node in $alldependentAssemblyNodes)
{
   if($node.assemblyIdentity.name -eq 'System.Net.Http')
   {
        $node.bindingRedirect.newVersion='4.0.0.0';
   }
}
$xml.Save($path);
