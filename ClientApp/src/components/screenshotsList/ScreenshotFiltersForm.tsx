import React, { useEffect, useRef } from "react";
import { Grid, Box, Button, Chip } from "@mui/material";
import { FormikProps } from "formik";
import TextField from "../form/TextField";
import SelectField from "../form/SelectField";
import { SearchScope } from "../../behavior/types";

type Props = {
    formik: FormikProps<FilterForm>;
    categories: { id: string; name: string; color: string }[] | undefined;
    onSubmit: () => void;
};

export type FilterForm = {
    query: string;
    searchScope: SearchScope;
    categoryIds: string[];
};

const ScreenshotFiltersForm = ({ formik, categories, onSubmit }: Props) => {
    const debounceRef = useRef<ReturnType<typeof setTimeout> | null>(null);

    // 🔍 debounce query
    useEffect(() => {
        debounceRef.current && clearTimeout(debounceRef.current)

        debounceRef.current = setTimeout(() => {
            onSubmit();
        }, 200);

        return () => {
            debounceRef.current && clearTimeout(debounceRef.current)
        }
    }, [formik.values.query]);

    // ⚡ instant filters
    useEffect(() => {
        if(!formik.values.query)
            return;

        debounceRef.current && clearTimeout(debounceRef.current)
        onSubmit();
    }, [formik.values.searchScope]);

    useEffect(() => {
        debounceRef.current && clearTimeout(debounceRef.current)
        onSubmit();
    }, [formik.values.categoryIds]);
    return (
        <Grid container spacing={2} mb={2}>
            <Grid size={{ xs: 12, md: 4 }}>
                <TextField name="query" label="Search" slotProps={{ htmlInput: { maxLength: 150 } }} />
            </Grid>

            <Grid size={{ xs: 12, md: 4 }}>
                <SelectField
                    name="searchScope"
                    label="Scope"
                    options={[
                        { label: "Title & Description", value: SearchScope.All },
                        { label: "Title", value: SearchScope.Title },
                    ]}
                    fullWidth
                />
            </Grid>

            <Grid size={{ xs: 12, md: 4 }}>
                <Box display="flex" flexWrap="wrap" gap={1}>
                    {categories?.map(cat => {
                        const selected = formik.values.categoryIds.includes(cat.id);

                        return (
                            <Chip
                                key={cat.id}
                                label={cat.name}
                                onClick={() => {
                                    const next = selected
                                        ? formik.values.categoryIds.filter(id => id !== cat.id)
                                        : [...formik.values.categoryIds, cat.id];

                                    formik.setFieldValue("categoryIds", next);
                                }}
                                sx={{
                                    backgroundColor: selected ? cat.color : undefined,
                                    color: selected ? "#fff" : undefined
                                }}
                            />
                        );
                    })}
                </Box>
            </Grid>
        </Grid>
    );
};

export default React.memo(ScreenshotFiltersForm);