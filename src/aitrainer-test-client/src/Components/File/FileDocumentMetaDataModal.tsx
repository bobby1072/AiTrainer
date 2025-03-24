import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  Grid2,
  TextField,
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
      <DialogContent dividers>
        <Grid2
          container
          justifyContent="center"
          alignItems="center"
          direction={"row"}
          width="100%"
          spacing={2}
        >
          {metaData.title && (
            <Grid2 width={"45%"}>
              <TextField
                fullWidth
                label="Title"
                value={metaData.title}
                disabled
              />
            </Grid2>
          )}
          {metaData.author && (
            <Grid2 width={"45%"}>
              <TextField
                fullWidth
                label="Author"
                value={metaData.author}
                disabled
              />
            </Grid2>
          )}
          {metaData.subject && (
            <Grid2 width={"45%"}>
              <TextField
                fullWidth
                label="Subject"
                value={metaData.subject}
                disabled
              />
            </Grid2>
          )}
          {metaData.keywords && (
            <Grid2 width={"45%"}>
              <TextField
                fullWidth
                label="Keywords"
                value={metaData.keywords}
                disabled
              />
            </Grid2>
          )}
          {metaData.creator && (
            <Grid2 width={"45%"}>
              <TextField
                fullWidth
                label="Creator"
                value={metaData.creator}
                disabled
              />
            </Grid2>
          )}
          {metaData.producer && (
            <Grid2 width={"45%"}>
              <TextField
                fullWidth
                label="Producer"
                value={metaData.producer}
                disabled
              />
            </Grid2>
          )}
          {metaData.creationDate && (
            <Grid2 width={"45%"}>
              <TextField
                fullWidth
                label="Creation date"
                value={metaData.creationDate}
                disabled
              />
            </Grid2>
          )}
          {metaData.modifiedDate && (
            <Grid2 width={"45%"}>
              <TextField
                fullWidth
                label="Modified date"
                value={metaData.modifiedDate}
                disabled
              />
            </Grid2>
          )}

          {metaData.numberOfPages && (
            <Grid2 width={"45%"}>
              <TextField
                fullWidth
                label="Number of pages"
                value={metaData.numberOfPages}
                disabled
              />
            </Grid2>
          )}

          {(metaData.isEncrypted !== null ||
            metaData.isEncrypted !== undefined) && (
            <Grid2 width={"45%"}>
              <TextField
                fullWidth
                label="Is encrypted"
                value={metaData.isEncrypted}
                disabled
              />
            </Grid2>
          )}
        </Grid2>
      </DialogContent>
      <DialogActions>
        <Grid2
          container
          justifyContent="center"
          alignItems="center"
          width="100%"
        >
          <Grid2>
            <Button variant="outlined" color="primary" onClick={onClose}>
              Cancel
            </Button>
          </Grid2>
        </Grid2>
      </DialogActions>
    </Dialog>
  );
};
