import React, { useEffect, useState } from "react";
import { useDeleteFileCollectionMutation } from "../../Hooks/useDeleteFileCollectionMutation";
import { FileCollection } from "../../Models/FileCollection";
import {
  Box,
  ButtonBase,
  IconButton,
  TableCell,
  TableRow,
  Tooltip,
  Typography,
} from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import { prettyDateWithTime } from "../../Utils/DateTime";
import { useSnackbar } from "notistack";
import { MenuPosition } from "../Contexts/FileCollectionContextMenuContext";
import { SaveFileCollectionModal } from "./SaveFileCollectionModal";
const fileCol = require("./fileCol.png");

export const NewFileTableCollectionRow: React.FC<{
  fileCollection: FileCollection;
  handleRightClick: (event: React.MouseEvent) => void;
  closeContextMenu: () => void;
  menuPosition: MenuPosition | null;
}> = ({ fileCollection, closeContextMenu, handleRightClick, menuPosition }) => {
  const [isCollectionSaveModalOpen, setIsCollectionSaveModalOpen] =
    useState<boolean>(false);
  const { mutate, isLoading, data } = useDeleteFileCollectionMutation();
  const { enqueueSnackbar } = useSnackbar();

  useEffect(() => {
    if (data) {
      enqueueSnackbar(`Collection deleted`, { variant: "error" });
    }
  }, [data, enqueueSnackbar]);

  const dateCreated = new Date(fileCollection.dateCreated);
  const dateModified = new Date(fileCollection.dateModified);
  return (
    <>
      <TableRow onContextMenu={handleRightClick} onClick={closeContextMenu}>
        <TableCell>
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              gap: 3,
            }}
          >
            <Box
              component="img"
              sx={{
                width: "3%",
              }}
              src={fileCol}
              alt={`fileColImage: ${fileCollection.id}`}
            />
            <ButtonBase
              href={`/collection/home/${fileCollection.id}`}
              sx={{
                "&:hover": {
                  textDecoration: "underline",
                },
              }}
            >
              <Typography>{fileCollection.collectionName}</Typography>
            </ButtonBase>
          </Box>
        </TableCell>
        <TableCell align="right">
          <Tooltip title={`${dateCreated.toISOString()}`}>
            <Typography>{prettyDateWithTime(dateCreated)}</Typography>
          </Tooltip>
        </TableCell>
        <TableCell align="right">
          <Tooltip title={`${dateModified.toISOString()}`}>
            <Typography>{prettyDateWithTime(dateModified)}</Typography>
          </Tooltip>
        </TableCell>
        <TableCell align="right" />
        <TableCell align="right">
          <IconButton
            color="inherit"
            size="small"
            disabled={isLoading}
            onClick={() => {
              mutate({ id: fileCollection.id! });
            }}
          >
            <DeleteIcon />
          </IconButton>
        </TableCell>
      </TableRow>
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
              setIsCollectionSaveModalOpen(true);
              closeContextMenu();
            }}
          >
            Rename
          </li>
        </ul>
      )}
      {isCollectionSaveModalOpen && (
        <SaveFileCollectionModal
          closeModal={() => setIsCollectionSaveModalOpen(false)}
          fileCollInput={{
            collectionName: fileCollection.collectionName,
            parentId: fileCollection.parentId,
            id: fileCollection.id,
            dateCreated: fileCollection.dateCreated,
            dateModified: fileCollection.dateModified,
          }}
        />
      )}
    </>
  );
};
