import React, { memo } from 'react';
import {
    Stack
} from '@mui/material';

import TextFieldWrapper from '../form/TextField';
import SwitchField from '../form/SwitchField';
import ColorPicker from '../form/ColorPicker';

export interface ModalSectionProps {
    highlightEnabled: boolean
}

const highlightWordName = "highlightWord"

const HighlightWordSection = ({
    highlightEnabled,
}: ModalSectionProps) => {

    return (
        <>
            <SwitchField
                name='highlightEnabled'
                label="Enable Highlight Word"
            />
            {highlightEnabled && (
                <Stack spacing={2} sx={{ ml: 2 }}>
                    <TextFieldWrapper maxLength={200} name={`${highlightWordName}.word`} label="Word to Highlight" fullWidth />
                    <ColorPicker name={`${highlightWordName}.color`} label="Highlight Color" placeholder="#FFFF00" fullWidth />
                </Stack>
            )}
        </>
    );
};

export default memo(HighlightWordSection);