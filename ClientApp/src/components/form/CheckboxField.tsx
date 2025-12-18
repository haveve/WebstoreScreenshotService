import { Field } from "formik";
import {
    Checkbox,
    FormControlLabel,
    FormHelperText,
    FormControl,
} from "@mui/material";
import { FieldProps } from "./types";
import { formatValidate } from "./helpers";

const CheckboxField = ({ name, label, schema, ...props }: FieldProps) => {
    return (
        <Field name={name} validate={formatValidate(schema)}>
            {({ field, meta }: any) => (
                <FormControl error={meta.touched && Boolean(meta.error)}>
                    <FormControlLabel
                        control={
                            <Checkbox
                                {...field}
                                {...props}
                                checked={field.value}
                            />
                        }
                        label={label}
                    />
                    {meta.touched && meta.error && (
                        <FormHelperText>{meta.error}</FormHelperText>
                    )}
                </FormControl>
            )}
        </Field>
    );
};

export default CheckboxField;
