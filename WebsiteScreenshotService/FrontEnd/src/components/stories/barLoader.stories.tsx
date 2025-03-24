import { BarLoader } from 'react-spinners';
import { Meta } from '@storybook/react';
import 'bootstrap/dist/css/bootstrap.min.css';

const meta: Meta<typeof BarLoader> = {
    title: 'BarLoader',
    component: BarLoader,
    argTypes: {
        color: {
            type: 'string',
            description: 'Color of the loader',
            defaultValue: '#3498db',
            control: {
                type: 'color',
            },
        },
        width: {
            type: 'string',
            description: 'Width of the loader',
            defaultValue: '100%',
            control: { type: 'text' },
        },
        height: {
            type: 'string',
            description: 'Height of the loader',
            defaultValue: '4px',
            control: { type: 'text' },
        },
    },
};

export default meta;

export const Default = {
    args: {
        color: '#3498db',
        width: '100%',
        height: '4px',
    }
}

export const RedLoader = {
    args: {
        color: '#e74c3c',
        width: '100%',
        height: '4px',
    }
}

export const LargeLoader = {
    args: {
        color: '#2ecc71',
        width: '100%',
        height: '8px',
    }
}
