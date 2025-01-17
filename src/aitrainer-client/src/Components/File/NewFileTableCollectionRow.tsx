import React, { useEffect } from "react";
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
import { useNavigate } from "react-router-dom";
const fileCol = require("./fileCol.png");

export const NewFileTableCollectionRow: React.FC<{
  fileCollection: FileCollection;
  handleRightClick: (
    event: React.MouseEvent,
    fileCollection: FileCollection
  ) => void;
  closeContextMenu: () => void;
  menuPosition: MenuPosition | null;
}> = ({ fileCollection, closeContextMenu, handleRightClick, menuPosition }) => {
  const { mutate, isLoading, data } = useDeleteFileCollectionMutation();
  const { enqueueSnackbar } = useSnackbar();
  const navigate = useNavigate();
  useEffect(() => {
    if (data) {
      enqueueSnackbar(`Collection deleted`, { variant: "error" });
    }
  }, [data, enqueueSnackbar]);

  const dateCreated = new Date(fileCollection.dateCreated);
  const dateModified = new Date(fileCollection.dateModified);
  return (
    <>
      <TableRow
        onContextMenu={(e) => handleRightClick(e, fileCollection)}
        onClick={closeContextMenu}
      >
        <TableCell
          sx={{
            maxWidth: "200px",
            wordWrap: "break-word",
            whiteSpace: "normal",
            textOverflow: "ellipsis",
          }}
        >
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
                width: "10%",
              }}
              src={fileCol}
              alt={`fileColImage: ${fileCollection.id}`}
            />
            <ButtonBase
              href={`/collection/home/${fileCollection.id}`}
              onClick={(e) => {
                e.preventDefault();
                navigate(`/collection/home/${fileCollection.id}`);
              }}
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
        <TableCell
          align="left"
          sx={{
            wordWrap: "break-word",
            whiteSpace: "normal",
            maxWidth: "500px",
          }}
        >
          {fileCollection.collectionDescription}
        </TableCell>
        <TableCell align="right">
          <Tooltip title={`UTC: ${dateCreated.toISOString()}`}>
            <Typography>{prettyDateWithTime(dateCreated)}</Typography>
          </Tooltip>
        </TableCell>
        <TableCell align="right">
          <Tooltip title={`UTC: ${dateModified.toISOString()}`}>
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
    </>
  );
};
