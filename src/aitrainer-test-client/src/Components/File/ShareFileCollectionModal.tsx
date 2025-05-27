import { z } from "zod";
import { useShareFileCollectionWithMembersMutation } from "../../Hooks/useShareFileCollectionWithMembersMutation";
import { useSnackbar } from "notistack";
import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

const sharedFileCollectionSingleMemberSaveInputSchema = z.object({
  userId: z.string().uuid(),
  canViewDocuments: z.boolean(),
  canDownloadDocuments: z.boolean(),
  canCreateDocuments: z.boolean(),
  canRemoveDocuments: z.boolean(),
});

export type SharedFileCollectionSingleMemberSaveInput = z.infer<
  typeof sharedFileCollectionSingleMemberSaveInputSchema
>;

const mapDefaultValues = (
  userId: string
): Partial<SharedFileCollectionSingleMemberSaveInput> => {
  return {
    userId,
    canViewDocuments: false,
    canDownloadDocuments: false,
    canCreateDocuments: false,
    canRemoveDocuments: false,
  };
};

export const ShareFileCollectionModal: React.FC<{
  closeModal: () => void;
  collectionId: string;
}> = ({ closeModal, collectionId }) => {
  const {
    handleSubmit,
    register,
    setValue,
    watch,
    formState: { errors: formErrors },
  } = useForm<SharedFileCollectionSingleMemberSaveInput>({
    defaultValues: mapDefaultValues(collectionId),
    resolver: zodResolver(sharedFileCollectionSingleMemberSaveInputSchema),
  });

  const { mutate, reset, isLoading, error, data } =
    useShareFileCollectionWithMembersMutation();
  const { enqueueSnackbar } = useSnackbar();
  useEffect(() => {
    if (data) {
      closeModal();
      enqueueSnackbar("Document added successfully", { variant: "success" });
    }
  }, [data, closeModal, enqueueSnackbar]);

  return null;
};
