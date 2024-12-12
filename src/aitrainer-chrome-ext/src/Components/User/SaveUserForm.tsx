import { z } from "zod";
import { User } from "../../Models/User";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useConfirmUser } from "../Hooks/ConfirmUser";
import { Button, Grid2, TextField } from "@mui/material";
import { ErrorComponent } from "../Common/ErrorComponent";
import { useGetDeviceToken } from "../Hooks/GetDeviceToken";
import { SolicitedDeviceToken } from "../../Models/SolicitedDeviceToken";

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

export const SaveUserForm: React.FC<{ user?: User | null }> = ({ user }) => {
  const {
    register,
    handleSubmit,
    formState: { errors: formErrors, isDirty },
  } = useForm<SaveUserFormInput>({
    defaultValues: mapDefaultValues(user),
    resolver: zodResolver(userFormSchema),
  });

  const {
    isLoading,
    error,
    confirmUser,
    reset: resetMutation,
  } = useConfirmUser({
    onSuccess: (successData, successVariables, successContext) => {
      const clone = { ...deviceToken };
      clone.inUse = true;
      setDeviceToken(clone as SolicitedDeviceToken);
    },
  });
  const { setValue: setDeviceToken, data: deviceToken } = useGetDeviceToken();

  return (
    <form
      id="save-user-form"
      onSubmit={handleSubmit((submitVals) => {
        resetMutation();
        confirmUser(submitVals);
      })}
    >
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
