import React, { Component } from 'react';

export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
      <div className='home-page'>
        <h1>Flashpoint Submissions</h1>
        <p>If you don't know what this site is, head to our website <a href='https://bluemaxima.org/flashpoint/'>here</a> first </p>
        <p>If you're new to curating, be sure to read the wiki articles below:</p>
        <p>Curation Tutorial: <a href='https://bluemaxima.org/flashpoint/datahub/Curation_Tutorial'>https://bluemaxima.org/flashpoint/datahub/Curation_Tutorial</a></p>
        <p>Curation Format: <a href='https://bluemaxima.org/flashpoint/datahub/Curation_Format'>https://bluemaxima.org/flashpoint/datahub/Curation_Format</a></p>
        <p>If you're here to submit a curation, please login via the top right button to proceed</p>
      </div>
    );
  }
}
