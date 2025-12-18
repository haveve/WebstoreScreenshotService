import { FieldArray, useFormikContext, getIn, FormikErrors, FormikTouched } from 'formik';
import { Box, Stack, Button, Typography, IconButton } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';
import { useTranslation } from 'react-i18next';

type ArrayFieldWrapperProps<T = any> = {
    name: string;
    label: string;
    emptyValue: T;
    children: (params: {
        value: T;
        index: number;
        error?: FormikErrors<T>;
        touched?: FormikTouched<T>;
        parentName: string;
    }) => React.ReactNode;
};

const ArrayFieldWrapper = <T,>({ name, label, children, emptyValue }: ArrayFieldWrapperProps<T>) => {
    const { values, errors, touched } = useFormikContext<any>();
    const { t } = useTranslation();
    const arrayValues: T[] = values[name] || [];

    const arrayErrors = getIn(errors, name) as FormikErrors<T>[] | undefined;
    const arrayTouched = getIn(touched, name) as FormikTouched<T>[] | undefined;

    return (
        <Box sx={{ mt: 2, mb: 2 }}>
            <Typography variant="subtitle1" gutterBottom>{label}</Typography>
            <FieldArray name={name}>
                {({ push, remove }) => (
                    <Stack spacing={1}>
                        {arrayValues.map((item, index) => {
                            const itemError = arrayErrors?.[index];
                            const itemTouched = arrayTouched?.[index];
                            return (
                                <Stack direction="row" spacing={1} alignItems="flex-start" key={index}>
                                    {children({
                                        value: item,
                                        index,
                                        error: itemError,
                                        touched: itemTouched,
                                        parentName: name,
                                    })}

                                    <Stack justifyContent="center">
                                        <IconButton color="error" onClick={() => remove(index)}>
                                            <DeleteIcon />
                                        </IconButton>
                                    </Stack>
                                </Stack>
                            );
                        })}
                        <Button variant="outlined" startIcon={<AddIcon />} onClick={() => push(emptyValue)}>
                            {t('General.add', { item: label })}
                        </Button>
                    </Stack>
                )}
            </FieldArray>
        </Box>
    );
};

export default ArrayFieldWrapper;
