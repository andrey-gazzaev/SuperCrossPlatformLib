import { dotnet } from './_framework/dotnet.js'

const { getAssemblyExports, getConfig, runMain } = await dotnet
    .withApplicationArguments("start")
    .create();

const config = getConfig();
const exports = await getAssemblyExports(config.mainAssemblyName);

export default exports;

// run the C# Main() method and keep the runtime process running and executing further API calls
await runMain();