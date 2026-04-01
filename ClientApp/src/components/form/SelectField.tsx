import { Field } from "formik";
import {
  MenuItem,
  Select,
  FormControl,
  InputLabel,
  FormHelperText,
  SelectProps as BaseSelectProps
} from "@mui/material";
import { FieldProps } from "./types";
import { memo, ReactNode } from "react";

/**
 * Wrapper for MUI TextField
 */

type SelectFieldProps = BaseSelectProps & {
  name: string;
  label: ReactNode;
  options: { label: string; value: any }[];
}

const SelectField = ({ name, label, options, ...props }: SelectFieldProps) => {
  return (
    <FormControl>
      <InputLabel>{label}</InputLabel>
      <Field name={name}>
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

export default memo(SelectField);
