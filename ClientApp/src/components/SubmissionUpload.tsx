import { plainToClass } from 'class-transformer';
import * as React from 'react';
import { uploadFile } from '../services/SubmissionService';
import { Submission } from '../types/Submission';
import sizeToString from 'filesize';

export type SubmissionUploadBoxProps = {
  onSubmissionUploaded?: (sub: Submission) => void;
}
export type SubmissionsBoxState = {
  selectedFiles?: FileList;
  progressInfos: ProgressInfo[];
  message: string;
}

type ProgressInfo = {
  progress: number;
  fileName: string;
  size: string;
}

export class SubmissionsUploadBox extends React.Component<SubmissionUploadBoxProps, SubmissionsBoxState>{

  constructor(props: SubmissionUploadBoxProps) {
    super(props);
    
    this.state = {
      progressInfos: [],
      message: 'Select a file to upload'
    }
  }

  selectFiles = (files: FileList | null) => {
    this.setState({
      selectedFiles: files || undefined
    });
  }

  upload = () => {
    const { selectedFiles } = this.state;
    if (selectedFiles) {
      const progressInfos: ProgressInfo[] = [];

      for (let i = 0; i < selectedFiles.length; i++) {
        progressInfos.push({ progress: 0, fileName: selectedFiles[i].name, size: sizeToString(selectedFiles[i].size) });
      }

      this.setState({
        progressInfos
      });

      for (let i = 0; i < selectedFiles.length; i++) {
        uploadFile(i, selectedFiles[i], (event) => {
          this.updateProgess(i, Math.round((100 * event.loaded) / event.total));
        })
        .then((res) => {
          if (this.props.onSubmissionUploaded) {
            for (const sub of res.data) {
              this.props.onSubmissionUploaded(plainToClass(Submission, sub));
            }
          }
          this.setState({
            message: `File Uploaded - ${selectedFiles[i].name}`
          });
        })
        .catch((err) => {
          this.setState({
            message: `Could not upload the file! ${err}`
          });
        });
      }
      this.setState({ selectedFiles: undefined });
    }
  }

  updateProgess = (idx: number, progress: number) => {
    const newProgresses: ProgressInfo[] = [...this.state.progressInfos];
    newProgresses[idx].progress = progress;
    this.setState({
      progressInfos: newProgresses
    })
  }

  render() {
    const { progressInfos } = this.state;
    return (
      <div className='submissions-upload'>
        <div className='submissions-upload__upload'>
          <label className="btn btn-default">
            <input type="file" multiple onChange={e => this.selectFiles(e.target.files)} />
          </label>

          <button
            className="btn btn-success"
            disabled={!this.state.selectedFiles}
            onClick={this.upload}
          >
            Upload
          </button>

          <div className="alert" role="alert">
            {this.state.message}
          </div>
        </div>

        <div className='submissions-upload__progress-container'>
          { progressInfos.map((p) => (
            <div className='submissions-progress'>
              <p>{p.fileName} - {p.size}</p>
              <div className="progress">
                <div
                  className="progress-bar progress-bar-info progress-bar-striped"
                  role="progressbar"
                  aria-valuenow={p.progress}
                  aria-valuemin={0}
                  aria-valuemax={100}
                  style={{ width: p.progress + "%" }}
                >
                  {p.progress}%
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    );
  }
}