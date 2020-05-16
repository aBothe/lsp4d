using System.Collections.Generic;
using System.Reactive.Subjects;

namespace D_Parserver.DHandler.Resolution
{
    public class WorkspaceManager
    {
        public static readonly BehaviorSubject<List<string>> WorkspaceFolders 
            = new BehaviorSubject<List<string>>(new List<string>());
    }
}