# Why Do We Need the Smart.Credential.Retriever ?

The **Smart.Credential.Retriever** workflow is essential in environments where multiple robots access the same application but cannot share the same credentials simultaneously. In many enterprise applications, attempting to use the same credentials for concurrent sessions can result in one or more robots being logged out automatically. This often leads to process failures, as the application detects multiple logins from the same account and terminates conflicting sessions.

Without a solution like this, a credential being used by one robot could unintentionally be assigned to another robot, causing the first robot's session to be forcibly logged out. This creates critical issues, such as:
- **Failed automations** due to unexpected logouts.
- **Inconsistent behavior** where robots cannot complete their tasks.
- **Increased downtime** as processes need to be restarted.

The **GetAvailableCredential** workflow dynamically manages credential allocation, ensuring that only unused credentials are assigned to robots. By checking the Credential Manager, it prevents the assignment of credentials that are already in use. This guarantees that:
- Each robot operates with a unique and free credential.
- Application session conflicts are avoided.
- Automation processes run smoothly without interruption.

This solution is indispensable for ensuring reliable and uninterrupted robotic process automation (RPA), especially when dealing with applications that enforce strict login policies.

# How it Works:

The program entry starts from the **GetAvailableCredential** workflow is designed to identify and retrieve an available credential asset from a specified Orchestrator folder. It accomplishes this by comparing a list of predefined asset credential names against those currently stored in the Credential Manager, which serves as a cache for the credentials in use.

The **Credential Manager** is essentially an asset that temporarily stores used credential asset names and job IDs in JSON text format as the value of the asset. Job IDs uniquely identify a running process and can be retrieved using the Get Job activity.

To find an available credential, the workflow iterates through the list of asset names and checks each one to determine if it is currently in use. It selects the first asset name that is absent from the Credential Manager, ensuring that it is not being utilized by any other processes.

Once an available credential asset name is identified, the workflow retrieves the corresponding username and password using a UiPath Get Credential activity. Following this, the job ID and the name of the used credential asset are registered in the Credential Manager. This registration process prevents other robots that reference this program from using the same credential, thereby ensuring that only free and unused assets are selected for operations.

## Workflow Steps

This workflow serves as the entry point for the program, where the following actions occur:

1. **Default Folder Names**: 
   - If the variable `sharedOrchestratorFolderName` is not declared, the program defaults to "Shared."
   - Similarly, if `topLevelOrchestratorFolderName` is not declared, it defaults to "/", representing the top-level tenant folder. 
   - The `CredentialManager` asset is utilized to cache the credentials that are currently in use.

2. **ValidateListOfCredentialAssetNamesProvided **: 
   -This workflow checks the validity of a list of credential asset names provided. It verifies that each asset name in the provided list meets exist in the Orchestrator.

3. **CreateCredentialManagerAssetIfNotExisting**: 
   - This workflow creates a text value asset named `CredentialManager` in the specified Orchestrator folder if it does not already exist. 
   - Upon its initial creation, the asset's default value will be a JSON string: `{"CredentialName":"JobId"}`. This format indicates that it will store a JSON object containing the properties `CredentialName` and `JobId`.

4. **ReadAndUpdateCredentialManager**: 
   - This workflow gets a list of active jobs from the Credential Manager Asset, where job information is stored as JSON text. 
   - First, it uses the Orchestrator API to find all jobs that are currently running or pending, which are called active jobs.
   - For each active job, if the job ID is found in the Credential Manager's JSON text, it is kept. If there are older jobs that are no longer active, they are removed from the JSON text. 
   - Finally, the updated JSON text is saved back to the Credential Manager Asset and returned in a Credential Manager Dictionary.

5. **Iterate Asset Names**: 
   - The program iterates through all credential asset names retrieved in (1), checking for its absence in the Credential Manager Dictionary from (4). 
   - Upon finding an asset that is not currently in use (i.e., not present in Credential Manager Dictionary), the program captures its name and exits the loop. 
   - This asset name represents an available resource ready for use.

If step (5) successfully returns a credential asset name, then (6) and (7) are executed:

6. **GetCredentialActivity**: 
   - This workflow uses the retrieved asset name to invoke the UiPath `GetCredential` activity, fetching the corresponding credential (username and password).

7. **AddCurrentJobIdToCredentialManager**: 
   - This workflow registers the job ID of the current process with the asset name in the Credential Manager. 
   - It updates the associated value in the JSON text format. For instance, if the job ID is `56sg672628-67126gh1jj2`, the JSON text will be updated to `{"CredentialName":"JobId","Credential1":"asxdf"}`. 
   - This ensures that any subsequent programs that access this library cannot use `Credential1` while the job ID `[asxdf]` is still running or pending.

8. **Error Handling**: 
   - If step (5) does not return a credential asset name, the program throws an error.


# How To Use In Uipath
After  downloading package into your project from the Manage Depencies on your uipath studio
### Key Arguments You Need to Supply:

1. **`topLevelOchestratorFolderName`**
   - **What it is**: The name of the top-level folder in Orchestrator.
   - **Example**: `"Company"`
   - If not declared, it defaults to `/`, representing the top-level tenant folder.

2. **`sharedOchestratorFolderName`**
   - **What it is**: The specific folder inside the top-level folder where the credentials are stored.
   - **Example**: `"IT_Credentials"`
   - If not declared, the program defaults to `"Shared."`

3. **`listOfCredentialAssetNamesInSharedFolder`**
   - **What it is**: A list of the names of the credentials you want to retrieve from that folder.
   - **Example**: `{"AppCredentials1", "AppCredentials2", "AppCredentials3"}`

### How to Use These:

- If your folder structure in Orchestrator looks like this:
  - Top-level folder: `"Company"`
  - Shared folder: `"General"`
  - Credential assets you want to always choose from: `"AppCredentials1"`, `"AppCredentials2"`, `"AppCredentials3"`

You would pass these values as arguments like this:
- **`topLevelOchestratorFolderName`**: `"Company"`
- **`sharedOchestratorFolderName`**: `"General"`
- **`listOfCredentialAssetNamesInSharedFolder`**: `{"AppCredentials1", "AppCredentials2", "AppCredentials3"}`

This is all you need, and the library would return the credentials which are not in use by another process or robot.
```
