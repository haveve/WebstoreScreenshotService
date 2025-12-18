import { Field } from "formik";
import {
  MenuItem,
  Select,
  FormControl,
  InputLabel,
  FormHelperText
} from "@mui/material";
import { FieldProps } from "./types";
import { formatValidate } from "./helpers";

type SelectFieldProps = {
  options: { label: string; value: any }[];
} & FieldProps;

const SelectField = ({ name, label, options, schema, ...props }: SelectFieldProps) => {
  return (
    <FormControl>
      <InputLabel>{label}</InputLabel>
      <Field name={name} validate={formatValidate(schema)}>
        {({ field, meta }: FieldProps) => (
          <>
            <Select
              {...field}
              {...props}
              label={label}
              value={field.value}
              error={meta.touched && Boolean(meta.error)}
            >
              {options.map((opt, index) => (
                <MenuItem key={index} value={opt.value}>
                  {opt.label}
                </MenuItem>
              ))}
            </Select>
            {meta.touched && meta.error && (
              <FormHelperText>{meta.error}</FormHelperText>
            )}
          </>)}
      </Field>
    </FormControl>

  );
};

export default SelectField;
