import React from 'react';

export type LoginLogoutButtonProps = {
  loggedIn: boolean;
}

export function LoginLogoutButton(props: LoginLogoutButtonProps) {
  return (
    <form>
      <button
        className={`btn ${props.loggedIn ? 'btn-danger' : 'btn-success'}`}
        formAction={props.loggedIn ? '/logout' : '/login'}
      >
        {props.loggedIn ? 'Logout' : 'Login'}
      </button>
    </form>
  );
}