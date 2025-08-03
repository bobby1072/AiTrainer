import { Button, Grid2, Paper, TextField, Typography } from "@mui/material";
import { useChatQueryMutation } from "../../Hooks/useChatQueryMutation";
import { AnalyseChunkInReferenceToQuestionQueryInput } from "../../Models/AnalyseChunkInReferenceToQuestionQueryInput";
import { useState } from "react";
import { SingleDocumentChunk } from "../../Models/SingleDocumentChunk";
import { ErrorComponent } from "../Common/ErrorComponent";
import { ChatFormattedQueryInput } from "../../Models/ChatFormattedQueryInput";

export const SingleFaissResponseItemTab: React.FC<{
  responseItem: SingleDocumentChunk;
  collectionId?: string | null;
}> = ({ responseItem, collectionId }) => {
  const { data, error, mutate, reset, isLoading } =
    useChatQueryMutation<AnalyseChunkInReferenceToQuestionQueryInput>();
  const [question, setQuestion] = useState<string>();

  const askQuestion = (
    vals: ChatFormattedQueryInput<AnalyseChunkInReferenceToQuestionQueryInput>
  ) => {
    reset();
    mutate(vals);
  };
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
            <Grid2 width={"88%"}>
              <TextField
                fullWidth
                placeholder="Enter further questions about this information..."
                value={question}
                onChange={(e) => setQuestion(e.target.value)}
              />
            </Grid2>
            <Grid2 width={"10%"}>
              <Button
                disabled={!question || isLoading}
                variant="contained"
                color="primary"
                sx={{
                  height: "100%",
                }}
                onClick={() =>
                  askQuestion({
                    definedQueryFormatsEnum: 1,
                    queryInput: {
                      collectionId,
                      chunkId: responseItem.id,
                      question: question!,
                    },
                  })
                }
              >
                Ask
              </Button>
            </Grid2>
            {data && (
              <Grid2
                width={"100%"}
                sx={{
                  textAlign: "left",
                  padding: 3,
                }}
              >
                <Typography variant="subtitle2">{data}</Typography>
              </Grid2>
            )}
            {error && (
              <Grid2>
                <ErrorComponent errorMessage={error.message} />
              </Grid2>
            )}
          </Grid2>
        </Grid2>
      </Grid2>
    </Paper>
  );
};
