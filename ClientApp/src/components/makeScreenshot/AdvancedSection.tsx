import {
    Stack
} from '@mui/material';

import TextFieldWrapper from '../form/TextField';
import SelectFieldWrapper from '../form/SelectField';
import ArrayFieldWrapper from '../form/ArrayField';

import {
    HeaderModel, CookieModel, ColorSchemeOption
} from '../../behavior/types';
import SwitchField from '../form/SwitchField';
import { memo } from 'react';

export interface ModalSectionProps {
    advancedEnabled: boolean
}

const AdvancedSection = ({
    advancedEnabled,
}: ModalSectionProps) => {

    return (
        <>
            <SwitchField
                name='advancedEnabled'
                label="Enable Advanced Configuration"
            />
            {advancedEnabled && (<>
                <Stack spacing={2} sx={{ ml: 2 }}>
                    <TextFieldWrapper name="locale" label="Locale (en-US)" fullWidth />
                    <TextFieldWrapper name="timezoneId" label="Timezone (Europe/Kyiv)" fullWidth />
                    <SelectFieldWrapper
                        name="colorScheme"
                        label="Color Scheme"
                        options={[
                            { value: ColorSchemeOption.Light, label: 'Light' },
                            { value: ColorSchemeOption.Dark, label: 'Dark' },
                            { value: ColorSchemeOption.NoPreference, label: 'No Preference' }
                        ]}
                    />
                    <TextFieldWrapper maxLength={200} name="waitForSelector" label="Wait For Selector" fullWidth />
                    <SelectFieldWrapper
                        name="blockResources"
                        label="Block Resources"
                        multiple
                        options={[
                            { value: 'Images', label: 'Images' },
                            { value: 'Fonts', label: 'Fonts' },
                            { value: 'Media', label: 'Media' },
                            { value: 'Scripts', label: 'Scripts' },
                            { value: 'Stylesheets', label: 'Stylesheets' }
                        ]}
                    />

                    {/* Headers */}
                    <ArrayFieldWrapper<HeaderModel>
                        name="headers"
                        label="Headers"
                        emptyValue={{ name: '', value: '' }}
                        maxLength={20}>
                        {({ index, parentName }) => (
                            <Stack direction="row" key={index} spacing={1} sx={{ width: '100%' }}>
                                <TextFieldWrapper maxLength={100} name={`${parentName}[${index}].name`} label="Header Name" fullWidth />
                                <TextFieldWrapper maxLength={1000} name={`${parentName}[${index}].value`} label="Header Value" fullWidth />
                            </Stack>
                        )}
                    </ArrayFieldWrapper>

                    {/* Cookies */}
                    <ArrayFieldWrapper<CookieModel>
                        name="cookies"
                        label="Cookies"
                        emptyValue={{
                            name: '',
                            value: '',
                            domain: '',
                            path: '/',
                            secure: false,
                            httpOnly: false,
                            expires: new Date().toISOString().slice(0, 16)
                        }}
                        maxLength={15}
                    >
                        {({ index, parentName }) => (
                            <Stack key={index} spacing={1} sx={{ width: '100%', p: 2, border: '1px solid #ddd', borderRadius: 1 }}>
                                <Stack direction="row" spacing={1}>
                                    <TextFieldWrapper maxLength={100} name={`${parentName}[${index}].name`} label="Cookie Name" fullWidth />
                                    <TextFieldWrapper value={1000} name={`${parentName}[${index}].value`} label="Cookie Value" fullWidth />
                                </Stack>
                                <Stack direction="row" spacing={1}>
                                    <TextFieldWrapper maxLength={200} name={`${parentName}[${index}].domain`} label="Domain" placeholder=".example.com" fullWidth />
                                    <TextFieldWrapper maxLength={100} name={`${parentName}[${index}].path`} label="Path" placeholder="/" fullWidth />
                                </Stack>
                                <TextFieldWrapper
                                    type="datetime-local"
                                    label="Expires (UTC)"
                                    name={`${parentName}[${index}].expires`}
                                    inputProps={{
                                        min: new Date().toISOString().slice(0, 16), // now
                                        max: new Date(new Date().setMonth(new Date().getMonth() + 6))
                                            .toISOString()
                                            .slice(0, 16) // max 6 months
                                    }}
                                    fullWidth
                                />
                                <SelectFieldWrapper
                                    fullWidth
                                    name={`${parentName}[${index}].sameSite`}
                                    label="Same site"
                                    options={[
                                        { value: 'Strict', label: 'Strict' },
                                        { value: 'Lax', label: 'Lax' },
                                        { value: 'None', label: 'None' }
                                    ]} />
                                <Stack direction="row" spacing={2}>
                                    <SwitchField
                                        name={`${parentName}[${index}].secure`}
                                        label="Secure"
                                    />
                                    <SwitchField
                                        name={`${parentName}[${index}].httpOnly`}
                                        label="HttpOnly"
                                    />
                                </Stack>
                            </Stack>
                        )}
                    </ArrayFieldWrapper>
                </Stack>
            </>
            )}
        </>
    );
};

export default memo(AdvancedSection);