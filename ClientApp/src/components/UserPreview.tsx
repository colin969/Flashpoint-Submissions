import { User } from "../types/User";
import React from 'react';
import { RiShieldStarFill } from 'react-icons/ri';
import { IconType } from "react-icons";

export type UserPreviewProps = {
  user: User;
}

export function UserPreview(props: UserPreviewProps) {
  return (
    <div className='user-preview'>
      <div className='user-preview__username'>{props.user.username || props.user.email || 'No Username'}</div>
      { props.user.roles.map((role, idx) => {
        const render = getIconForRole(role);
        if (render) {
          return (
            <div key={idx} className='user-preview__role'>
              {render}
            </div>
          );
        }
      })}
      <img className='user-preview__icon' src={props.user.imageUrl} />
    </div>
  )
}

function getIconForRole(role: string): JSX.Element | undefined {
  switch(role) {
    case 'staff':
      return <RiShieldStarFill />;

  }
}