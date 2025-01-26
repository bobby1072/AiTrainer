import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  Grid2,
} from "@mui/material";
import { FileDocumentMetaData } from "../../Models/FileDocumentMetaData";
import { StyledDialogTitle } from "../Common/StyledDialogTitle";

export const FileDocumentMetaDataModal: React.FC<{
  metaData: FileDocumentMetaData;
  onClose: () => void;
}> = ({ metaData, onClose }) => {
  return (
    <Dialog open onClose={onClose}>
      <StyledDialogTitle title="File document meta data" />
      <DialogContent dividers></DialogContent>
      <DialogActions>
        <Grid2
          container
          justifyContent="center"
          alignItems="center"
          direction={"row"}
          width="100%"
        >
          <Grid2 width={"100%"}>
            <Button variant="outlined" color="primary" onClick={onClose}>
              Cancel
            </Button>
          </Grid2>
        </Grid2>
      </DialogActions>
    </Dialog>
  );
};
