import cv2
import numpy as np
import dlib
import time

from pylsl import StreamInfo, StreamOutlet, StreamInlet

cap = cv2.VideoCapture(0)

detector = dlib.get_frontal_face_detector()
predictor = dlib.shape_predictor("shape_predictor_68_face_landmarks.dat")

length = 24
info = StreamInfo('Certus', 'NDI', length)
outlet = StreamOutlet(info)

while True:
    _, frame = cap.read()
    gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY) 
    faces = detector(gray)
    for face in faces:
        MarkersPositions = []
        MarkerIndexes = [19,24,33,51,57,8,48,54]
        landmarks = predictor(gray, face)
        for n in range(0, 68):
            x = landmarks.part(n).x
            y = landmarks.part(n).y
            cv2.circle(frame, (x, y), 4, (255, 0, 0), -1) 
            cv2.putText(frame, "%d"%(n), (x, y), cv2.FONT_HERSHEY_SIMPLEX, 0.4, (0,0,0)) 
        for index in MarkerIndexes:    
            x = landmarks.part(index).x
            y = landmarks.part(index).y
            MarkersPositions.append(x)
            MarkersPositions.append(y)
            MarkersPositions.append(0)
        cv2.imshow("Frame", frame)
        outlet.push_sample(MarkersPositions)
        time.sleep(0.02)
        key = cv2.waitKey(1)
        if key == 27:
            break