import React, { Component } from 'react';

export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
      <div>
        <h1>Flashpoint Submissions</h1>
        <p>If you're new to curating, be sure to read the wiki articles INSERT HERE</p>
        <p>If you're here to submit a curation, please login via the top right button to proceed</p>
      </div>
    );
  }
}
