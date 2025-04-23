import { Button, Grid2, Paper, TextField, Typography } from "@mui/material";
import { SimilaritySearchResponseItem } from "../../Models/SimilaritySearchResponse";
import { useChatQueryMutation } from "../../Hooks/useChatQueryMutation";
import { AnalyseChunkInReferenceToQuestionQueryInput } from "../../Models/AnalyseChunkInReferenceToQuestionQueryInput";
import { useState } from "react";

export const SingleFaissResponseItemTab: React.FC<{
  responseItem: SimilaritySearchResponseItem;
}> = ({ responseItem }) => {
  const { data, error, mutate, isLoading } =
    useChatQueryMutation<AnalyseChunkInReferenceToQuestionQueryInput>();
  const [question, setQuestion] = useState<string>();
  return (
    <Paper
      elevation={1}
      sx={{
        border: "1px solid #ccc",
        borderRadius: "8px",
        overflow: "hidden",
      }}
    >
      <Grid2 container direction="column" spacing={2} padding={1} width="100%">
        <Grid2
          width={"100%"}
          sx={{
            textAlign: "left",
          }}
        >
          <Typography variant="subtitle2">
            {responseItem.pageContent}
          </Typography>
        </Grid2>

        <Grid2 width={"100%"}>
          <Grid2 container direction="row" spacing={1} width="100%">
            <Grid2 width={"90%"}>
              <TextField
                fullWidth
                placeholder="Enter further questions about this information..."
                value={question}
                onChange={(e) => setQuestion(e.target.value)}
              />
            </Grid2>
            <Grid2 width={"10%"}>
              <Button>Ask</Button>
            </Grid2>
          </Grid2>
        </Grid2>
      </Grid2>
    </Paper>
  );
};
