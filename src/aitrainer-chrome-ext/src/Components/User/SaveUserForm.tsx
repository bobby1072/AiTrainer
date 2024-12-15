import { z } from "zod";
import { User } from "../../Models/User";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Button, Grid2, TextField } from "@mui/material";
import { ErrorComponent } from "../Common/ErrorComponent";

const userFormSchema = z.object({
  id: z.string().uuid().optional().nullable(),
  email: z.string().email(),
  name: z.string(),
});

type SaveUserFormInput = z.infer<typeof userFormSchema>;

const mapDefaultValues = (user?: User | null): Partial<SaveUserFormInput> => {
  return {
    email: user?.email,
    name: user?.name,
  };
};

export const SaveUserForm: React.FC<{
  user?: User | null;
  handleSubmit: (x: SaveUserFormInput) => void;
  isLoading: boolean;
  error?: Error | null;
}> = ({ user, handleSubmit: userHandlerFunc, isLoading, error }) => {
  const {
    register,
    handleSubmit,
    formState: { errors: formErrors, isDirty },
  } = useForm<SaveUserFormInput>({
    defaultValues: mapDefaultValues(user),
    resolver: zodResolver(userFormSchema),
  });
  return (
    <form id="save-user-form" onSubmit={handleSubmit(userHandlerFunc)}>
      <Grid2
        container
        justifyContent="center"
        alignItems="center"
        direction="column"
        spacing={2}
        width="100%"
      >
        <Grid2 width="100%">
          <TextField
            {...register("email", { required: true })}
            label="Email"
            fullWidth
            multiline
            rows={2}
            error={!!formErrors.email}
            helperText={formErrors.email?.message}
          />
        </Grid2>
        <Grid2 width="100%">
          <TextField
            {...register("name", { required: true })}
            label="Name"
            fullWidth
            multiline
            rows={2}
            error={!!formErrors.email}
            helperText={formErrors.email?.message}
          />
        </Grid2>
        {error && (
          <Grid2 width="100%">
            <ErrorComponent errorMessage={error.message} />
          </Grid2>
        )}
        <Grid2 width="100%">
          <Button
            disabled={!isDirty || isLoading}
            type="submit"
            variant="contained"
            color="primary"
          >
            Save
          </Button>
        </Grid2>
      </Grid2>
    </form>
  );
};
