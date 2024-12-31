import { createContext, useContext, useState } from "react";
import { FileCollection } from "../../Models/FileCollection";
import { SaveFileCollectionModal } from "../File/SaveFileCollectionModal";

export type MenuPosition = {
  x: number;
  y: number;
};

export const useFileCollectionContextMenuContext = () => {
  const context = useContext(FileCollectionContextMenuContext);
  if (!context)
    throw new Error(
      `${FileCollectionContextMenuContext.displayName} has not been registered`
    );
  return context;
};

export type FileCollectionContextMenuContextType = {
  handleRightClick: (
    event: React.MouseEvent,
    fileCollection: FileCollection
  ) => void;
  closeContextMenu: () => void;
  menuPosition: MenuPosition | null;
};

export const FileCollectionContextMenuContext = createContext<
  FileCollectionContextMenuContextType | undefined
>(undefined);

export const FileCollectionContextMenuContextProvider: React.FC<{
  children: React.ReactNode;
}> = ({ children }) => {
  const [menuPosition, setMenuPosition] = useState<MenuPosition | null>(null);
  const [fileCollectionToEdit, setFileCollectionToEdit] =
    useState<FileCollection>();
  const [isCollectionSaveModalOpen, setIsCollectionSaveModalOpen] =
    useState<boolean>(false);
  const handleRightClick = (
    event: React.MouseEvent,
    fileCollection: FileCollection
  ) => {
    event.preventDefault();
    setMenuPosition({
      x: event.pageX,
      y: event.pageY,
    });
    setFileCollectionToEdit(fileCollection);
  };

  const handleClick = () => {
    setMenuPosition(null);
  };

  return (
    <div onClick={handleClick}>
      <FileCollectionContextMenuContext.Provider
        value={{
          handleRightClick,
          closeContextMenu: handleClick,
          menuPosition,
        }}
      >
        {children}
      </FileCollectionContextMenuContext.Provider>
      {menuPosition && (
        <ul
          style={{
            position: "absolute",
            top: menuPosition.y,
            left: menuPosition.x,
            backgroundColor: "#fff",
            border: "1px solid #ccc",
            padding: "10px",
            boxShadow: "0px 4px 6px rgba(0, 0, 0, 0.1)",
            listStyleType: "none",
            zIndex: 1000,
          }}
        >
          <li
            style={{ padding: "5px 10px", cursor: "pointer" }}
            onClick={() => {
              handleClick();
              setIsCollectionSaveModalOpen(true);
            }}
          >
            Rename
          </li>
        </ul>
      )}
      {fileCollectionToEdit && isCollectionSaveModalOpen && (
        <SaveFileCollectionModal
          closeModal={() => setIsCollectionSaveModalOpen(false)}
          fileCollInput={{
            collectionName: fileCollectionToEdit.collectionName,
            collectionDescription: fileCollectionToEdit.collectionDescription,
            parentId: fileCollectionToEdit.parentId,
            id: fileCollectionToEdit.id,
            dateCreated: fileCollectionToEdit.dateCreated,
            dateModified: fileCollectionToEdit.dateModified,
          }}
        />
      )}
    </div>
  );
};
