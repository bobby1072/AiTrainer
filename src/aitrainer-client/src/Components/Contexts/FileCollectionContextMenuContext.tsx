import { createContext, useContext, useState } from "react";

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
  handleRightClick: (event: React.MouseEvent) => void;
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

  const handleRightClick = (event: React.MouseEvent) => {
    event.preventDefault();
    setMenuPosition({
      x: event.pageX,
      y: event.pageY,
    });
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
    </div>
  );
};
