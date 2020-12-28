import React from 'react';

export type LoginLogoutButtonProps = {
  loggedIn: boolean;
}

export function LoginLogoutButton(props: LoginLogoutButtonProps) {
  return (
    <form>
      <a
        type='button'
        className={`btn ${props.loggedIn ? 'btn-danger' : 'btn-success'}`}
        href={props.loggedIn ? '/logout' : '/login'}
      >
        {props.loggedIn ? 'Logout' : 'Login'}
      </a>
    </form>
  );
}
