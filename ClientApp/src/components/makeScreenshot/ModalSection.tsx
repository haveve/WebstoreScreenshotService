import { memo } from 'react';
import {
    Stack
} from '@mui/material';

import TextFieldWrapper from '../form/TextField';
import ArrayFieldWrapper from '../form/ArrayField';
import SwitchField from '../form/SwitchField';

export interface ModalSectionProps {
    modalEnabled: boolean
}

const modelName = "modal"

const ModalSection = ({
    modalEnabled,
}: ModalSectionProps) => {

    return (
        <>
            <SwitchField
                name='modalEnabled'
                label="Enable Modal Settings"
            />
            {modalEnabled && (
                <Stack spacing={2} sx={{ ml: 2 }}>
                    <SwitchField
                        name={`${modelName}.dismissDialogs`}
                        label="Dismiss Dialogs"
                    />
                    <SwitchField
                        name={`${modelName}.hidePopups`}
                        label="Hide Popups"
                    />
                    <ArrayFieldWrapper<string>
                        name={`${modelName}.hideSelectors`}
                        label="Hide Selectors"
                        emptyValue=""
                        maxLength={15}>
                        {({ index, parentName }) => (
                            <TextFieldWrapper maxLength={200} key={index} name={`${parentName}[${index}]`} label={`Selector ${index + 1}`} fullWidth />
                        )}
                    </ArrayFieldWrapper>
                </Stack>
            )}
        </>
    );
};

export default memo(ModalSection);