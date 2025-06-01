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
import { useNavigate } from "react-router-dom";
import IosShareIcon from "@mui/icons-material/IosShare";
import { ShareFileCollectionModal } from "./ShareFileCollectionModal";

const fileCol = require("./fileCol.png");

export const ActualFileCollectionTableCollectionRow: React.FC<{
  fileCollection: FileCollection;
  handleRightClick: (
    event: React.MouseEvent,
    fileCollection: FileCollection
  ) => void;
  closeContextMenu: () => void;
  singleLevel: boolean;
}> = ({ fileCollection, closeContextMenu, handleRightClick, singleLevel }) => {
  const {
    mutate: deleteFileMutation,
    isLoading: deleteFileIsLoading,
    data: deleteFileData,
    error: deleteFileError,
  } = useDeleteFileCollectionMutation();
  const [shareFileCollectionModalOpen, setShareFileCollectionModalOpen] =
    useState<boolean>(false);

  const { enqueueSnackbar } = useSnackbar();
  const navigate = useNavigate();
  useEffect(() => {
    if (deleteFileData) {
      enqueueSnackbar(`Collection successfully deleted`, {
        variant: "success",
      });
    }
  }, [deleteFileData, enqueueSnackbar]);
  useEffect(() => {
    if (deleteFileError) {
      enqueueSnackbar("Failed to delete collection", { variant: "error" });
    }
  }, [deleteFileError, enqueueSnackbar]);

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
                if (!singleLevel) {
                  navigate(`/collection/home/${fileCollection.id}`);
                }
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
            onClick={() => setShareFileCollectionModalOpen(true)}
          >
            <IosShareIcon />
          </IconButton>
        </TableCell>
        <TableCell align="right">
          <IconButton
            color="inherit"
            size="small"
            disabled={deleteFileIsLoading}
            onClick={() => {
              deleteFileMutation({ id: fileCollection.id! });
            }}
          >
            <DeleteIcon />
          </IconButton>
        </TableCell>
      </TableRow>
      {shareFileCollectionModalOpen && (
        <ShareFileCollectionModal
          closeModal={() => setShareFileCollectionModalOpen(false)}
          collectionId={fileCollection.id!}
        />
      )}
    </>
  );
};
