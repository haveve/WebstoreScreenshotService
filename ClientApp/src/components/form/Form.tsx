import { ReactNode } from 'react';
import { Formik, Form, FormikHelpers, FormikValues, FormikProps } from 'formik';
import * as Yup from 'yup';
import { memo } from "react";

type FormikFormWrapperProps<T extends FormikValues> = {
  initialValues: T;
  validateOnChange?: boolean;
  validationSchema?: Yup.ObjectSchema<any>; // optional Yup schema
  onSubmit: (values: T, helpers: FormikHelpers<T>) => void | Promise<void>;
  children: (formProps: FormikProps<T>) => ReactNode;
};

const FormikFormWrapper = <T extends FormikValues>({
  initialValues,
  validationSchema,
  validateOnChange,
  onSubmit,
  children,
}: FormikFormWrapperProps<T>) => {
  return (
    <Formik<T>
      initialValues={initialValues}
      validationSchema={validationSchema}
      validateOnChange={validateOnChange}
      onSubmit={onSubmit}
    >
      {(props) => <Form>{children(props)}</Form>}
    </Formik>
  );
};

export default memo(FormikFormWrapper) as typeof FormikFormWrapper;
