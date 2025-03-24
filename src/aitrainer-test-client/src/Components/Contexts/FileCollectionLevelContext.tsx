import { createContext, useContext } from "react";
import { usePersistedState } from "../../Hooks/usePersistedState";

export type FileCollectionLevelContextType = {
  fileColId: string;
  setFileColId: (fileColId: string) => void;
};

export const FileCollectionLevelContext = createContext<
  FileCollectionLevelContextType | undefined
>(undefined);

export const useFileCollectionLevelContext = () => {
  const context = useContext(FileCollectionLevelContext);
  if (!context)
    throw new Error(
      `${FileCollectionLevelContext.displayName} has not been registered`
    );
  return context;
};

export const FileCollectionLevelContextProvider: React.FC<{
  children: React.ReactNode;
}> = ({ children }) => {
  const [fileColId, setFileColId] = usePersistedState<string>(
    FileCollectionLevelContext.displayName || "FileCollectionLevelContext",
    ""
  );

  return (
    <FileCollectionLevelContext.Provider value={{ fileColId, setFileColId }}>
      {children}
    </FileCollectionLevelContext.Provider>
  );
};
